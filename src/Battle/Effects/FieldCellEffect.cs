using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyvatar.src.Battle.Effects
{
    public abstract class FieldCellEffect
    {
        public bool ToBeDeleted;

        public abstract void Update();
        public abstract void Draw();
    }
}
