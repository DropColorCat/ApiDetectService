using detection.entity;

namespace detection.communication
{
    public interface IBaseDetection
    {

        bool proprocess(int extension);

        bool checkCommunication(int extension);

        bool startDetect(int extension);

        void timeout();

        DetectData receiveData(int extension, int group);
        
    }
}
