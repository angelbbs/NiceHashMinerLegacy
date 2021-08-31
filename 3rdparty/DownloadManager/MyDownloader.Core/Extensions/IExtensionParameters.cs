using System.ComponentModel;

namespace MyDownloader.Core.Extensions
{
    public interface IExtensionParameters
    {
        event PropertyChangedEventHandler ParameterChanged;
    }
}
