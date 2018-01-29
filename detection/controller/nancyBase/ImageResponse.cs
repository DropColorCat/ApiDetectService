using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace detection.controller.nancyBase
{
    public class ImageResponse : Nancy.Response
    {
        public ImageResponse(byte[] buffer, string contentType = "image/jpg")
        {
            try
            {
                if (buffer == null || buffer.Length == 0)
                {
                    this.StatusCode = Nancy.HttpStatusCode.NotFound;
                    return;
                }
                Contents = (stream) =>
                {
                    try
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    catch (Exception)
                    {
                        // loging
                    }
                };
                ContentType = contentType;
            }
            catch (Exception)
            {
                // loging               
            }
        }
    }
}
