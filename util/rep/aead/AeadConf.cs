using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using util.crypt;
using util.crypt.sodium;
using util.ext;

namespace util.rep.aead
{
    public class AeadConf
    {
        public string Type = AeadReposit.Type;
        public int Version = AeadReposit.Version + 1;
        public string Encode = "utf-8,b64";
        public int MasterKeySize = 64;
        public int FileIdSize = 16;
        public int BlockSize = 4.kb();
        public KeyGenEntry[] KeyGenes;
        public KeyDeriveType KeyDerive = KeyDeriveType.HKDF;
        public AeadCryptType DataCrypt = AeadCryptType.ChaCha20Poly1305;
        public DirCryptType DirCrypt = DirCryptType.HmacIvCbc;
        public byte[] Nonce;
        public byte[] Cipher;

        byte[] genPwdKey(byte[] pwd, int size)
        {
            var pwdKey = pwd ?? new byte[0];
            KeyGenes.each(kg => pwdKey = kg.genKey(pwdKey, size));
            return pwdKey;
        }

        byte[] mkey;
        public bool decrypt(byte[] pwd)
        {
            mkey = new byte[Cipher.Length - aead.TagSize];
            return getCrypt(pwd).decrypt(Cipher, 0, Cipher.Length, Nonce, mkey);
        }

        public AeadConf create()
        {
            mkey = new byte[MasterKeySize].aesRnd();
            return this;
        }

        public AeadConf encrypt(byte[] pwd)
        {
            Nonce = aead.NonceSize.aesRnd();
            Cipher = new byte[mkey.Length + aead.TagSize];
            getCrypt(pwd).encrypt(mkey, 0, mkey.Length, Nonce, Cipher);
            return this;
        }

        public AeadCrypt newCrypt()
            => util.crypt.AeadCrypt.create(DataCrypt);

        AeadCrypt ae;
        AeadCrypt aead => ae ?? (ae = newCrypt());
        AeadCrypt getCrypt(byte[] pwd)
        {
            var pkey = genPwdKey(pwd, aead.KeySize);
            return aead.setKey(pkey);
        }

        KeyDerive kdf;
        KeyDerive newKdf()
        {
            switch (KeyDerive)
            {
                case KeyDeriveType.HKDF:
                    return new HkdfDerive();
            }
            return null;
        }
        
        public byte[] deriveKey(byte[] ctx, int size)
            => (kdf ?? (kdf = newKdf())).derive(mkey, ctx, size);

        public DirCrypt newDirCrypt(byte[] ctx)
        {
            switch (DirCrypt)
            {
                case DirCryptType.HmacIvCbc:
                    var enc = new HmacIvCbcCrypt();
                    enc.Key = deriveKey(ctx, enc.KeySize);
                    return enc;
            }
            return null;
        }

        public int nonceSize() => aead.NonceSize;
        public int tagSize() => aead.TagSize;
        public int packSize() => BlockSize + tagSize();
    }

    public class KeyGenEntry
    {
        public KeyGenType type;
        public object args;

        KeyGen getArgs()
            => (args is KeyGen ? args : args = args.jcopy(getType()))
                as KeyGen;

        public byte[] genKey(byte[] pwd, int size)
            => getArgs().genKey(pwd, size);

        Type getType()
        {
            switch (type)
            {
                case KeyGenType.PBKDF2:
                    return typeof(PBKDF2);
                case KeyGenType.Argon2id:
                    return typeof(Argon2id);
            }
            return null;
        }
    }
}
