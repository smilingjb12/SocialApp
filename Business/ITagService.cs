using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace Business
{
    public interface ITagService
    {
        Tag GetOrCreateTag(string name);
    }
}
