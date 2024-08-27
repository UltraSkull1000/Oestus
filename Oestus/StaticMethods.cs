using System.Security.Cryptography;

namespace Oestus;
public static class OestusRNG
{
    public static int Next(int min, int max){        
        using (var srng = new SecureRandom(min, max)){
            return srng.Next();
        }
    }

    public static List<int> Next(int min, int max, int count){
        List<int> result = new List<int>();
        using (var srng = new SecureRandom(min, max)){
            for(int i = 0; i < count; i++){
                result.Add(srng.Next());
            }
        }
        return result;
    }

    public class SecureRandom : IDisposable{
        private RandomNumberGenerator rng = RandomNumberGenerator.Create();
        private int min, range;
        public SecureRandom(int emax = 2) => _init(0,emax);
        public SecureRandom(int min, int emax) => _init(min,emax); 

        private void _init (int min, int emax){
            this.min = min;
            if(min >= emax)
                throw new ArgumentOutOfRangeException($"{nameof(min)} cannot exceed {nameof(emax)}.");
            range = emax - min;
        }

        public int Next() {
            byte[] uint32Buffer = new byte[4];
            rng.GetBytes(uint32Buffer);
            uint nextUint = BitConverter.ToUInt32(uint32Buffer, 0);
            var res = min + ((int)(nextUint % range));
            return res;
        }

        public void Dispose(){
            rng.Dispose();
        }
    }
}
