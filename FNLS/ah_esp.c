#include <linux/init.h>
#include <linux/module.h>
#include <linux/kernel.h>
#include <linux/skbuff.h>
#include <linux/ip.h>                 
#include <linux/netfilter.h>
#include <linux/netfilter_ipv4.h>
#include <linux/netdevice.h>
#include <net/sock.h>
#include <net/ip.h>
#include <linux/crypto.h>
#include <linux/err.h>
#include <linux/scatterlist.h>
#include <linux/gfp.h>
#include <crypto/hash.h>
#include <linux/interrupt.h>

static struct nf_hook_ops nfho;
static struct nf_hook_ops outnfho;

struct crypto_blkcipher *tfm_esp;
struct crypto_hash *tfm_ah;

static unsigned char drop_ip[] = "10.5.19.170";
static unsigned char drop_ip2[] = "127.0.0.1";


struct esphdr{
	int nextheader;
	int padlen;
};

struct ahhdr{
	int nextheader;
	char hash[44];
};

/*crypto -- hmac sha-1
*/


void do_hash(char *key, size_t klen, char *data_in, size_t dlen, char *data_out, size_t outlen)
{
	struct scatterlist sg;
	//struct crypto_hash *tfm;
	struct hash_desc desc;
	static char output[1024];
	int i;
	int ret;
	void *hash_buf;
	int buf_len;
	int plen = 8;
	if(dlen == plen)
		buf_len = dlen;
	else{
		if(dlen%plen == 0)
			buf_len = dlen;
		else
			buf_len = (dlen/plen + 1)*plen;
	}	
	
	//tfm = crypto_alloc_hash("hmac(sha1)", 0, CRYPTO_ALG_ASYNC);
	printk("In hmac_sha1: got key=%s, klen=%d, data_in=%s, dlen=%d, buf len=%d\n", key, klen, data_in, dlen, buf_len);

	if (IS_ERR(tfm_ah)) {
		printk(KERN_ERR "failed to load transform : %ld\n", 
		       PTR_ERR(tfm_ah));
		return;
	}
	
	desc.tfm = tfm_ah;
	desc.flags = 0;
	
	if (crypto_hash_digestsize(tfm_ah) > sizeof(output)) {
		printk(KERN_ERR "digestsize(%u) > outputbuffer(%zu)\n",
		       crypto_hash_digestsize(tfm_ah), sizeof(output));
		return;
	}
	
	if(!(hash_buf=kzalloc(buf_len,GFP_ATOMIC))) {
		printk(KERN_ERR "hmac_sha1: failed to kzalloc hash_buf");
		return;
	}
	memcpy(hash_buf,data_in,dlen);
	sg_init_one(&sg,hash_buf,dlen);
	
	if(crypto_hash_setkey(tfm_ah,key,klen)){
		printk(KERN_ERR "hmac_sha1: crypto_hash_setkey failed\n");
		return;
	}
	
	if(dlen == plen){
		ret = crypto_hash_digest(&desc, &sg, dlen, data_out);
	}else{
		ret = crypto_hash_init(&desc);
		if(ret){
			printk("hash_init failed\n");
			return;
		}
		int pcount;
		for (pcount = 0; pcount < dlen; pcount += plen) {
			ret = crypto_hash_update(&desc, &sg, plen);
			if (ret){
				printk("hash update failed\n");
				return;
			}
		}
		
		ret = crypto_hash_final(&desc, data_out);
		if (ret){
			printk("hash final failed\n");
			return;
		}
		
	}
	//crypto_free_hash(tfm);
	return;
}
 
// crypto -- cbc(aes)

// encryption

int do_encrypt(char *key, size_t klen, char *data_in, size_t dlen, char **data_out, int *outlen, int enc)
{
	char *iv = kzalloc(17, GFP_ATOMIC);
	int iv_len = 16;
	//struct crypto_blkcipher *tfm;
	struct blkcipher_desc desc;
	struct scatterlist sg;
	struct scatterlist sg_out;
	
	int ret;
	void *hash_buf;
	void *out_buf;
	
	printk("In do_encrypt, got: key=%s, klen=%d, data_in=%s, dlen=%d, enc=%d\n", key, klen, data_in, dlen, enc);
	
	int buf_len;
	buf_len = dlen;
	if(dlen%iv_len != 0)
		buf_len = (dlen/iv_len + 1)*iv_len;
		
	*outlen = buf_len;
	*data_out = kzalloc(buf_len, GFP_ATOMIC);
	
	char *in_buf = kzalloc(buf_len, GFP_ATOMIC);
	__memcpy(in_buf, data_in, dlen);
	
	
	if (IS_ERR(tfm_esp)) {
		printk("failed to load transform \n");
		return -1;
	}
	
	if(!tfm_esp){
		printk("oops tfm is null \n");
		return -1;
	}
	
	printk("block cipher allocated \n");
	desc.tfm = tfm_esp;
	desc.flags = 0;
	
	
	ret = crypto_blkcipher_setkey(tfm_esp, key, klen);
	//printk("3- Returning from do_encrypt\n");
	//return 0;
	//ret = crypto_blkcipher_setkey(tfm, key, sizeof(key));
	if (ret) {
		printk("setkey() failed flags=%x\n",
						crypto_blkcipher_get_flags(tfm_esp));
		return -1;
	}
	
	printk("setkey() succeeded \n");
	
	if(!(hash_buf=kzalloc(iv_len,GFP_ATOMIC))) {
		printk(KERN_ERR "failed to kzalloc hash_buf\n");
		return -1;
	}
	printk("hash buf is allocated\n");
	
	
	if(!(out_buf=kzalloc(iv_len,GFP_ATOMIC))) {
		printk(KERN_ERR "failed to kzalloc out_buf\n");
		return;
	}
	printk("out buf is allocated\n");
	
	//memcpy(hash_buf,data_in,dlen);
	//printk("data copied to hash buf = %s\n", (char *)hash_buf);
	sg_init_one(&sg, hash_buf, iv_len);
	printk("sg initialized\n");
	
	sg_init_one(&sg_out, out_buf, iv_len);
	
	printk("scatterlists initialized\n");
	
	//iv_len = crypto_blkcipher_ivsize(tfm);
	
	
	memset(iv, 1, 16);
	crypto_blkcipher_set_iv(tfm_esp, iv, 16);

	
	if(!(&iv)){
		printk("iv init failed \n");
		return;
	}
	
	printk("set_iv done - %s\n", iv);
	
	if(enc == 1)
	{
		printk("encrypting ...\n");
		int pcount;
		for(pcount = 0; pcount < buf_len; pcount = pcount + iv_len){
			__memcpy(hash_buf, in_buf+pcount, iv_len);
			printk("encrypting .. %s\n", (char *)hash_buf);
			sg_init_one(&sg, hash_buf, iv_len);
			ret = crypto_blkcipher_encrypt(&desc, &sg_out, &sg, iv_len);
			if(ret){
				printk("encryption failed\n");
				return;
			}
			sg_copy_to_buffer(&sg_out, 1, (void *)((*data_out)+pcount), iv_len);
			
		}
			
	}
	else
	{
		printk("decrypting ...\n");
		int pcount;
		for(pcount = 0; pcount<buf_len; pcount = pcount+iv_len){
			__memcpy(hash_buf, in_buf+pcount, iv_len);
			ret = crypto_blkcipher_decrypt(&desc, &sg_out, &sg, iv_len);
			if(ret){
				printk("decryption failed\n");
				return;
			}
			sg_copy_to_buffer(&sg_out, 1, (void *)(*data_out+pcount), iv_len);
		}
			
	}
	printk("encrypt done, ret = %d\n", ret);
	int i;
	printk("data out = %s\n", *data_out);	
	
	//sg_copy_to_buffer(&sg_out, 1,(void *)data_out, dlen);	
	//printk("data out = %s\n", data_out);
	//__memcpy(data_out, data_in, dlen);
	return ret;
}

// Netfilter hook functions

unsigned int out_hook_func(unsigned int hooknum,
                     struct sk_buff *skb,
                     const struct net_device *in,
                     const struct net_device *out,
                     int (*okfn)(struct sk_buff *))
{
	struct sk_buff *sb = skb;
    struct iphdr *nh;
    //struct sock *sk;
 	
	char *s_ip = kzalloc(17, GFP_ATOMIC);
    char *d_ip = kzalloc(17, GFP_ATOMIC);
    
    nh = (struct iphdr *)skb_network_header(sb);
    sprintf(s_ip,"%d.%d.%d.%d",NIPQUAD(nh->saddr));
    sprintf(d_ip,"%d.%d.%d.%d",NIPQUAD(nh->daddr));
    
    if( ((strcmp(s_ip, drop_ip)==0) || (strcmp(s_ip, drop_ip2)==0)) && ((strcmp(d_ip, drop_ip)==0) || (strcmp(d_ip, drop_ip2)==0)) )
	{
		printk("@@@-- id = %d, protocol = %d --@@@\n", nh->id, nh->protocol);
		
		if(nh->protocol == 51)
		{
			printk("@@@--Post routing packet :: AH mode selected --@@@\n");
		  	
		  	int lin = skb_linearize(sb);
	  		if(lin == 0)
	  			printk("@@@-- skb is linearized successfully --@@@\n");
	  		else
	  			printk("@@@-- Trouble linearizing --@@@\n");
	  		
	  		nh = (struct iphdr *)kzalloc(sizeof(struct iphdr), GFP_ATOMIC);
		  	if(!nh){
		  		printk("@@@-- nh alloc failed --@@@\n");
		  		return NF_DROP;
		  	}
	   	   	__memcpy(nh, skb_network_header(sb), sizeof(struct iphdr));
		  	
	  		struct ahhdr *ah = (struct ahhdr *)skb_pull(sb, sizeof(struct iphdr));
	  		
	  		if(!ah){
	  			printk("@@@-- ah not allocated --@@@\n");
	  			kfree(nh);
	  			return NF_DROP;
	  		}
	  		
	  		printk("@@@-- id = %d --@@@\n", nh->id);
	  		printk("@@@-- nxthdr=%d, hash=%s --@@@\n", ah->nextheader, ah->hash);
	  		
	  		nh->protocol = ah->nextheader;
	  		nh->tot_len = htons(ntohs(nh->tot_len) - sizeof(struct ahhdr));
	  		
	  		printk("@@@ checksum 1 = %d @@@\n", nh->check);
		  	nh->check = 0;
	  		ip_send_check(nh);
	  		printk("@@@ checksum 2 = %d @@@\n", nh->check);
	  		
	  		skb_pull(sb, sizeof(struct ahhdr));
	  		int sblen = sb->len;
	  		char *mystring = kzalloc(sb->len, GFP_ATOMIC);
	  		if(!mystring){
	  			printk("@@@-- mystring alloc failed !!! --@@@\n");
	  			kfree(nh);
	  			return NF_DROP;
	  		}
	  		
	  		__memcpy(mystring, sb->data, sb->len);
	  		
	  		static char *key = "passphrase";
	  		char *hmac = kzalloc(44, GFP_ATOMIC);
	  		if(!hmac){
	  			printk("@@@-- hmac alloc failed !!! --@@@\n");
	  			kfree(nh);
	  			kfree(mystring);
	  			return NF_DROP;
	  		}
	  		
	  		do_hash(key, strlen(key), mystring, sblen, hmac, 40);
			
			hmac[20] = '\0';
	  		printk("@@@-- hmac = %s --@@@\n", hmac);
	  		
	  		if(strncmp(hmac, ah->hash, 20) == 0){
	  			printk("@@@-- hmac matchs ah->hash, accepting.. --@@@\n");
		  		//return NF_ACCEPT;
		  	}
		  	else{
		  		printk("@@@-- hmac does not match ah->hash, rejecting.. --@@@\n");
		  		kfree(nh);
	  			kfree(mystring);
	  			kfree(hmac);
		  		return NF_DROP;
		  	}
	  		
	  		skb_reset_transport_header(sb);
	  		
	  		struct iphdr *iph = (struct iphdr *)skb_push(sb, sizeof(struct iphdr));
	  		__memcpy(iph, nh, sizeof(struct iphdr));
	  		skb_reset_network_header(sb);
	  		
	  		kfree(nh);
	  		kfree(mystring);
	  		kfree(hmac);
	  		return NF_ACCEPT;
		}												// end of protocol 51 if
		else if(nh->protocol == 50)
		{
			printk("@@@--Post routing packet :: AH mode selected --@@@\n");
		  	
		  	int lin = skb_linearize(sb);
	  		if(lin == 0)
	  			printk("@@@-- skb is linearized successfully --@@@\n");
	  		else
	  			printk("@@@-- Trouble linearizing --@@@\n");
	  		
	  		nh = (struct iphdr *)kzalloc(sizeof(struct iphdr), GFP_ATOMIC);
		  	if(!nh){
		  		printk("@@@-- nh alloc failed --@@@\n");
		  		return NF_DROP;
		  	}
	   	   	__memcpy(nh, skb_network_header(sb), sizeof(struct iphdr));
		  	
	  		struct esphdr *esp = (struct esphdr *)skb_pull(sb, sizeof(struct iphdr));
	  		
	  		if(!esp){
	  			printk("@@@-- ah not allocated --@@@\n");
	  			kfree(nh);
	  			return NF_DROP;
	  		}
	  		
	  		printk("@@@-- id = %d --@@@\n", nh->id);
	  		printk("@@@-- nxthdr=%d, padlen=%d --@@@\n", esp->nextheader, esp->padlen);
	  		
	  		nh->protocol = esp->nextheader;
	  		int padlen = esp->padlen;

	  		nh->tot_len = htons(ntohs(nh->tot_len) - sizeof(struct esphdr) -padlen);
	  		
	  		printk("@@@-- checksum 1 = %d --@@@\n", nh->check);
		  	nh->check = 0;
	  		ip_send_check(nh);
	  		printk("@@@-- checksum 2 = %d --@@@\n", nh->check);
	  		
	  		skb_pull(sb, sizeof(struct esphdr));
	  		int sblen = sb->len;
	  		char *mystring = kzalloc(sb->len, GFP_ATOMIC);
	  		if(!mystring){
	  			printk("@@@-- mystring alloc failed !!! --@@@\n");
	  			kfree(nh);
	  			return NF_DROP;
	  		}
	  		
	  		__memcpy(mystring, sb->data, sb->len);
	  		
	  		char *key = key	= "\x01\x23\x45\x67\x89\xab\xcd\xef"
			  "\x55\x55\x55\x55\x55\x55\x55\x55"
			  "\xfe\xdc\xba\x98\x76\x54\x32\x10";
			int klen = 24;
			char *decrypted_data;
			int dec_len;
			
			int ret = do_encrypt(key, klen, mystring, sblen, &decrypted_data, &dec_len, 0);
			if(ret)
			{
				printk("@@@-- decryption failed, ret = %d --@@@\n", ret);
				kfree(nh);
				kfree(mystring);
				return NF_DROP;
			}
	  		
			printk("@@@-- sblen=%d, dec_len=%d, sb->len=%d, padlen=%d --@@@\n", sblen, dec_len, sb->len, padlen);
			printk("@@@-- decrypted data = %s --@@@\n", decrypted_data);
			
			skb_pull(sb, sblen);
			skb_push(sb, sblen-padlen);
			__memcpy(sb->data, decrypted_data, sblen-padlen);
			
	  		skb_reset_transport_header(sb);
	  		
	  		struct iphdr *iph = (struct iphdr *)skb_push(sb, sizeof(struct iphdr));
	  		__memcpy(iph, nh, sizeof(struct iphdr));
	  		skb_reset_network_header(sb);
	  		
	  		kfree(nh);
	  		kfree(mystring);
	  		kfree(decrypted_data);
	  		return NF_ACCEPT;
		}												// end of protocol 50 if
	}													// end of strcmp if
    kfree(s_ip);
	kfree(d_ip);
	return NF_ACCEPT;
}


unsigned int hook_func(unsigned int hooknum,
                     struct sk_buff *skb,
                     const struct net_device *in,
                     const struct net_device *out,
                     int (*okfn)(struct sk_buff *))
{
	struct sk_buff *sb = skb;
    struct iphdr *nh;
    struct sock *sk;
	
	char *s_ip = kzalloc(17, GFP_ATOMIC);
    char *d_ip = kzalloc(17, GFP_ATOMIC);
    
    nh = (struct iphdr *)skb_network_header(sb);
    sprintf(s_ip,"%d.%d.%d.%d",NIPQUAD(nh->saddr));
    sprintf(d_ip,"%d.%d.%d.%d",NIPQUAD(nh->daddr));
    
    if( ((strcmp(s_ip, drop_ip)==0) || (strcmp(s_ip, drop_ip2)==0)) && ((strcmp(d_ip, drop_ip)==0) || (strcmp(d_ip, drop_ip2)==0)) )
	{
		sk = (struct sock *)(sb->sk);
		
		if(!sk){
	  		printk("###-- outgoing packet :: sk is null --###\n"); 
	  		return NF_ACCEPT;
	  	}
	  	else
	  		printk("###-- security level = %d --###\n", sk->sk_sec_level);
	  	
	  	if(sk->sk_sec_level == 0)
		  	return NF_ACCEPT;
		
		if(sk->sk_sec_level == 1)
		{			
			printk("###--Local out packet :: AH mode selected --###\n");
			int headroom = skb_headroom(sb);
			int s = sizeof(struct ahhdr);
			
			if(s > headroom){
				printk("###-- Not enough headroom (headroom=%d, s=%d) --###\n", headroom, s);
				return NF_DROP;
			}
			
		  	int lin = skb_linearize(sb);
		  	if(lin == 0)
		  		printk("###-- skb is linearized successfully --###\n");
		  	else
		  		printk("###-- Trouble linearizing --###\n");  		
			
			nh = (struct iphdr *)kzalloc(sizeof(struct iphdr), GFP_ATOMIC);
	  		if(!nh){
	  			printk("###-- nh alloc failed!!! --###\n");
	  			return NF_DROP;
	  		}
	  		__memcpy(nh, skb_network_header(sb), sizeof(struct iphdr));
	  
     		printk("###-- id = %d --###\n", nh->id);
     		
			skb_pull(sb, sizeof(struct iphdr));
			int sblen = sb->len;
			char *mystring = kzalloc(sb->len, GFP_ATOMIC);
	  		if(!mystring){
	  			printk("###-- mystring alloc failed !!! --###\n");
	  			kfree(nh);
	  			return NF_DROP;
	  		}
	  		
	  		__memcpy(mystring, sb->data, sb->len);
	  		
	  		static char *key = "passphrase";
	  		char *hmac = kzalloc(44, GFP_ATOMIC);
	  		if(!hmac){
	  			printk("hmac not allocated\n");
	  			kfree(nh);
	  			kfree(mystring);
	  			return NF_DROP;
	  		}
	  		do_hash(key, strlen(key), mystring, sblen, hmac, 40);
	  		
	  		struct ahhdr *ah = (struct ahhdr *)skb_push(sb, sizeof(struct ahhdr));
	  		if(!ah)
	  		{
	  			printk("ah not allocated\n");
	  			kfree(nh);
	  			kfree(mystring);
	  			kfree(hmac);
	  			return NF_DROP;
	  		}
	  		
	  		ah->nextheader = nh->protocol;
	  		
	  		strncpy(ah->hash, hmac, 40);
			ah->hash[20] = '\0';
			
			nh->protocol = 51;
			nh->tot_len = htons(ntohs(nh->tot_len) + sizeof(struct ahhdr));
			printk("###-- id = %d --###\n", nh->id);
	  		printk("### -- nxthdr = %d, hash = %s --###\n", ah->nextheader, ah->hash);
	  		
	  		printk("### checksum 1 = %d ###\n", nh->check);
	  	
	  		nh->check = 0;
	  		ip_send_check(nh);
	  		printk("### checksum 2 = %d ###\n", nh->check);
	  		
	  		struct iphdr *iph = (struct iphdr *)skb_push(sb, sizeof(struct iphdr));
	  		__memcpy(iph, nh, sizeof(struct iphdr));
	  		skb_reset_network_header(sb);
			
			kfree(nh);
			kfree(mystring);
			kfree(hmac);
			return NF_ACCEPT;
		}													// end of sec level = 1 if
		else if(sk->sk_sec_level == 2)
		{
			printk("###--Local out packet :: ESP mode selected --###\n");
			int headroom = skb_headroom(sb);
			int s = sizeof(struct esphdr) + 15;
			
			if(s > headroom){
				printk("###-- Not enough headroom (headroom=%d, s=%d) --###\n", headroom, s);
				return NF_DROP;
			}
			
		  	int lin = skb_linearize(sb);
		  	if(lin == 0)
		  		printk("###-- skb is linearized successfully --###\n");
		  	else
		  		printk("###-- Trouble linearizing --###\n");  	
		  	
		  	nh = (struct iphdr *)kzalloc(sizeof(struct iphdr), GFP_ATOMIC);
	  		if(!nh){
	  			printk("###-- nh alloc failed!!! --###\n");
	  			return NF_DROP;
	  		}
	  		__memcpy(nh, skb_network_header(sb), sizeof(struct iphdr));
	  
     		printk("###-- id = %d --###\n", nh->id);
     		
     		skb_pull(sb, sizeof(struct iphdr));
			int sblen = sb->len;
			char *mystring = kzalloc(sb->len, GFP_ATOMIC);
	  		if(!mystring){
	  			printk("###-- mystring alloc failed !!! --###\n");
	  			kfree(nh);
	  			return NF_DROP;
	  		}
	  		
	  		__memcpy(mystring, sb->data, sb->len);
	  		
	  		char *key = key	= "\x01\x23\x45\x67\x89\xab\xcd\xef"
			  "\x55\x55\x55\x55\x55\x55\x55\x55"
			  "\xfe\xdc\xba\x98\x76\x54\x32\x10";
			int klen = 24;
			int ret;
			
			char *encrypted_data;
			int enc_len;
			
			ret = do_encrypt(key, klen, mystring, sblen, &encrypted_data, &enc_len, 1);
			if(ret)
			{
				printk("###-- encryption failed, ret=%d --###\n", ret);
				kfree(nh);
				kfree(mystring);
				return NF_DROP;
			}
			
			skb_pull(sb, sblen);
			skb_push(sb, enc_len);
			printk("###-- sblen = %d, enc_len=%d, sb->len=%d --###\n", sblen, enc_len, sb->len);
			printk("###-- encrypted data = %s --###\n", encrypted_data);
			__memcpy(sb->data, encrypted_data, enc_len);
		
			
			struct esphdr *esp = (struct esphdr *)skb_push(sb, sizeof(struct esphdr));
	  		if(!esp)
	  		{
	  			printk("esp not allocated\n");
	  			kfree(nh);
				kfree(mystring);
				kfree(encrypted_data);
	  			return NF_ACCEPT;
	  		}
	  		
	  		esp->nextheader = nh->protocol;
			esp->padlen = enc_len - sblen;
			
	  		
	  		nh->tot_len = htons(ntohs(nh->tot_len) + sizeof(struct esphdr) + enc_len - sblen);
	  		nh->protocol = 50;
	  		
	  		
	  		printk("### calculated len=%d --###\n", nh->tot_len);
	  		printk("### checksum 1 = %d ###\n", nh->check);
	  	
	  		nh->check = 0;
	  		ip_send_check(nh);
	  		printk("### checksum 2 = %d ###\n", nh->check);
	  		
	  		struct iphdr *iph = (struct iphdr *)skb_push(sb, sizeof(struct iphdr));
	  		__memcpy(iph, nh, sizeof(struct iphdr));
	  		skb_reset_network_header(sb);
			
			
			kfree(nh);
			kfree(mystring);
			kfree(encrypted_data);
			return NF_ACCEPT;
		}													// end of sec level = 2 if
		
	}														// end of strcmp if
	kfree(s_ip);
	kfree(d_ip);
	return NF_ACCEPT;
}


static int ah_esp_init(void)
{
	printk(KERN_ALERT "I bear a charmed life.\n");
	
	tfm_ah = crypto_alloc_hash("hmac(sha1)", 0, CRYPTO_ALG_ASYNC);
	tfm_esp = crypto_alloc_blkcipher("cbc(aes)", 0, CRYPTO_ALG_ASYNC);
	/* Fill in our hook structure */
  	nfho.hook     = hook_func;
 	/* Handler function */
  	nfho.hooknum  = 3; /* First for IPv4 */ // NF_IP_LOCAL_OUT
  	nfho.pf       = PF_INET;
  	nfho.priority = NF_IP_PRI_FIRST;   /* Make our func first */
  	
  	outnfho.hook     = out_hook_func;
 	/* Handler function */
  	outnfho.hooknum  = 1; /* First for IPv4 */	// NF_IP_LOCAL_IN
  	outnfho.pf       = PF_INET;
  	outnfho.priority = NF_IP_PRI_FIRST;   /* Make our func first */
  	
  	

  	nf_register_hook(&nfho);
	nf_register_hook(&outnfho);

	return 0;
}

static void ah_esp_exit(void)
{
	
	nf_unregister_hook(&nfho);
	nf_unregister_hook(&outnfho);
	
	crypto_free_hash(tfm_ah);
	crypto_free_blkcipher(tfm_esp); 
	printk(KERN_ALERT "Out, out, brief candle!\n");

}

module_init(ah_esp_init);
module_exit(ah_esp_exit);

MODULE_LICENSE("GPL");
MODULE_AUTHOR("ALOK");
