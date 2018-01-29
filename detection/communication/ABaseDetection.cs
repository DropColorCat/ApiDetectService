using detection.entity;

namespace detection.communication
{
    public abstract class ABaseDetection : IBaseDetection
    {
        
        public virtual bool proprocess(int extension)
        {
            return true;
        }

        public virtual bool checkCommunication(int extension)
        {
            return true;
        }

        public virtual bool startDetect(int extension)
        {
            return true;
        }

        public virtual void timeout()
        {
            ;
        }

        public virtual DetectData receiveData(int extension, int group)
        {
            return null;
        }
    }
}
