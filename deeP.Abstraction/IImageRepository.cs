using deeP.Abstraction.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deeP.Abstraction
{
    public interface IImageRepository
    {
        Task<string> StoreImageAsync(Stream jpgStream);
        Task<Stream> GetImageStreamAsync(string id);
    }
}
