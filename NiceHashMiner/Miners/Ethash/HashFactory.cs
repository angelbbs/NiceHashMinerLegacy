using System;

namespace HashLib
{
    public static class HashFactory
    {



        public static class Crypto
        {
            public static class SHA3
            {

                /*
                public static IHash CreateHamsi224()
                {
                    return new HashLib.Crypto.SHA3.Hamsi224();
                }

                public static IHash CreateHamsi256()
                {
                    return new HashLib.Crypto.SHA3.Hamsi256();
                }

                public static IHash CreateHamsi384()
                {
                    return new HashLib.Crypto.SHA3.Hamsi384();
                }

                public static IHash CreateHamsi512()
                {
                    return new HashLib.Crypto.SHA3.Hamsi512();
                }
                */
                /*
                /// <summary>
                /// 
                /// </summary>
                /// <param name="a_hash_size">224, 256, 384, 512</param>
                /// <returns></returns>
                public static IHash CreateHamsi(HashLib.HashSize a_hash_size)
                {
                    switch (a_hash_size)
                    {
                        case HashLib.HashSize.HashSize224: return CreateHamsi224();
                        case HashLib.HashSize.HashSize256: return CreateHamsi256();
                        case HashLib.HashSize.HashSize384: return CreateHamsi384();
                        case HashLib.HashSize.HashSize512: return CreateHamsi512();
                        default: throw new ArgumentException();
                    }
                }
                */
                public static IHash CreateKeccak224()
                {
                    return new HashLib.Crypto.SHA3.Keccak224();
                }

                public static IHash CreateKeccak256()
                {
                    return new HashLib.Crypto.SHA3.Keccak256();
                }

                public static IHash CreateKeccak384()
                {
                    return new HashLib.Crypto.SHA3.Keccak384();
                }

                public static IHash CreateKeccak512()
                {
                    return new HashLib.Crypto.SHA3.Keccak512();
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="a_hash_size">224, 256, 384, 512</param>
                /// <returns></returns>
                public static IHash CreateKeccak(HashLib.HashSize a_hash_size)
                {
                    switch (a_hash_size)
                    {
                        case HashLib.HashSize.HashSize224: return CreateKeccak224();
                        case HashLib.HashSize.HashSize256: return CreateKeccak256();
                        case HashLib.HashSize.HashSize384: return CreateKeccak384();
                        case HashLib.HashSize.HashSize512: return CreateKeccak512();
                        default: throw new ArgumentException();
                    }
                }
                /*
                public static IHash CreateLuffa224()
                {
                    return new HashLib.Crypto.SHA3.Luffa224();
                }

                public static IHash CreateLuffa256()
                {
                    return new HashLib.Crypto.SHA3.Luffa256();
                }

                public static IHash CreateLuffa384()
                {
                    return new HashLib.Crypto.SHA3.Luffa384();
                }

                public static IHash CreateLuffa512()
                {
                    return new HashLib.Crypto.SHA3.Luffa512();
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="a_hash_size">224, 256, 384, 512</param>
                /// <returns></returns>
                public static IHash CreateLuffa(HashLib.HashSize a_hash_size)
                {
                    switch (a_hash_size)
                    {
                        case HashLib.HashSize.HashSize224: return CreateLuffa224();
                        case HashLib.HashSize.HashSize256: return CreateLuffa256();
                        case HashLib.HashSize.HashSize384: return CreateLuffa384();
                        case HashLib.HashSize.HashSize512: return CreateLuffa512();
                        default: throw new ArgumentException();
                    }
                }

                public static IHash CreateShabal224()
                {
                    return new HashLib.Crypto.SHA3.Shabal224();
                }

                public static IHash CreateShabal256()
                {
                    return new HashLib.Crypto.SHA3.Shabal256();
                }

                public static IHash CreateShabal384()
                {
                    return new HashLib.Crypto.SHA3.Shabal384();
                }

                public static IHash CreateShabal512()
                {
                    return new HashLib.Crypto.SHA3.Shabal512();
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="a_hash_size">224, 256, 384, 512</param>
                /// <returns></returns>
                public static IHash CreateShabal(HashLib.HashSize a_hash_size)
                {
                    switch (a_hash_size)
                    {
                        case HashLib.HashSize.HashSize224: return CreateShabal224();
                        case HashLib.HashSize.HashSize256: return CreateShabal256();
                        case HashLib.HashSize.HashSize384: return CreateShabal384();
                        case HashLib.HashSize.HashSize512: return CreateShabal512();
                        default: throw new ArgumentException();
                    }
                }

                public static IHash CreateSHAvite3_224()
                {
                    return new HashLib.Crypto.SHA3.SHAvite3_224();
                }

                public static IHash CreateSHAvite3_256()
                {
                    return new HashLib.Crypto.SHA3.SHAvite3_256();
                }

                public static IHash CreateSHAvite3_384()
                {
                    return new HashLib.Crypto.SHA3.SHAvite3_384();
                }

                public static IHash CreateSHAvite3_512()
                {
                    return new HashLib.Crypto.SHA3.SHAvite3_512();
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="a_hash_size">224, 256, 384, 512</param>
                /// <returns></returns>
                public static IHash CreateSHAvite3(HashLib.HashSize a_hash_size)
                {
                    switch (a_hash_size)
                    {
                        case HashLib.HashSize.HashSize224: return CreateSHAvite3_224();
                        case HashLib.HashSize.HashSize256: return CreateSHAvite3_256();
                        case HashLib.HashSize.HashSize384: return CreateSHAvite3_384();
                        case HashLib.HashSize.HashSize512: return CreateSHAvite3_512();
                        default: throw new ArgumentException();
                    }
                }

                public static IHash CreateSIMD224()
                {
                    return new HashLib.Crypto.SHA3.SIMD224();
                }

                public static IHash CreateSIMD256()
                {
                    return new HashLib.Crypto.SHA3.SIMD256();
                }

                public static IHash CreateSIMD384()
                {
                    return new HashLib.Crypto.SHA3.SIMD384();
                }

                public static IHash CreateSIMD512()
                {
                    return new HashLib.Crypto.SHA3.SIMD512();
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="a_hash_size">224, 256, 384, 512</param>
                /// <returns></returns>
                public static IHash CreateSIMD(HashLib.HashSize a_hash_size)
                {
                    switch (a_hash_size)
                    {
                        case HashLib.HashSize.HashSize224: return CreateSIMD224();
                        case HashLib.HashSize.HashSize256: return CreateSIMD256();
                        case HashLib.HashSize.HashSize384: return CreateSIMD384();
                        case HashLib.HashSize.HashSize512: return CreateSIMD512();
                        default: throw new ArgumentException();
                    }
                }

                public static IHash CreateSkein224()
                {
                    return new HashLib.Crypto.SHA3.Skein224();
                }

                public static IHash CreateSkein256()
                {
                    return new HashLib.Crypto.SHA3.Skein256();
                }

                public static IHash CreateSkein384()
                {
                    return new HashLib.Crypto.SHA3.Skein384();
                }

                public static IHash CreateSkein512()
                {
                    return new HashLib.Crypto.SHA3.Skein512();
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="a_hash_size">224, 256, 384, 512</param>
                /// <returns></returns>
                public static IHash CreateSkein(HashLib.HashSize a_hash_size)
                {
                    switch (a_hash_size)
                    {
                        case HashLib.HashSize.HashSize224: return CreateSkein224();
                        case HashLib.HashSize.HashSize256: return CreateSkein256();
                        case HashLib.HashSize.HashSize384: return CreateSkein384();
                        case HashLib.HashSize.HashSize512: return CreateSkein512();
                        default: throw new ArgumentException();
                    }
                }
                */
            }


        }

    }
}
