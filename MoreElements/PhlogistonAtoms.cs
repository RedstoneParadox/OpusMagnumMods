using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtomTypes = class_175;
using Texture = class_256;

namespace Phlogiston
{
    internal static class PhlogistonAtoms
    {
        public static AtomType Phlogiston;

        public static void AddAtomTypes()
        {
            Phlogiston = new AtomType()
            {
                field_2283 = 86, /*ID*/
                field_2284 = class_134.method_254("Phlogiston"), /*Non-local Name*/
                field_2285 = class_134.method_253("Elemental Phlogiston", string.Empty), /*Atomic Name*/
                field_2286 = class_134.method_253("Phlogiston", string.Empty), /*Local name*/
                field_2287 = class_235.method_615("textures/atoms/leppa/UnstableElements/aether_symbol"), /*Symbol*/
                field_2288 = class_235.method_615("textures/atoms/leppa/UnstableElements/aether_shadow") /*Shadow*/
            };
        }
    }
}
