﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace util.crypt.sodium
{
    public class Argon2id : KeyGen
    {
        public Argon2id() => Sodium.checkLib();

        public const int SaltSize = 16;

        public byte[] salt;
        public long cpu = 32;
        public int mem = 64.mb();

        public override byte[] genKey(byte[] pwd, int keySize)
            => Sodium.argon2id(pwd, salt, cpu, mem, keySize);
    }
}
