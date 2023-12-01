using Quintessential;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AtomTypes = class_175;
using Texture = class_256;
using CardinalAtomColors = class_5;

namespace Phlogiston
{
    internal static class PhlogistonAtoms
    {
        public static AtomType Phlogiston;

        public static void AddAtomTypes()
        {
            Phlogiston = new AtomType()
            {
                field_2283 = 218, /*ID*/
                field_2293 = true, /*Is Cardinal*/
                field_2284 = class_134.method_254("Phlogiston"), /*Non-local Name*/
                field_2285 = class_134.method_253("Elemental Phlogiston", string.Empty), /*Atomic Name*/
                field_2286 = class_134.method_253("Phlogiston", string.Empty), /*Local name*/
                field_2287 = class_235.method_615("phlogiston/textures/atoms/phlogiston_symbol"), /*Symbol*/
                field_2288 = class_235.method_615("phlogiston/textures/atoms/phlogiston_shadow"), /*Shadow*/
                field_2289 = new CardinalAtomColors()
                {
                    field_8 = class_235.method_615("phlogiston/textures/atoms/phlogiston_base"),/*Base*/
                    field_9 = class_238.field_1989.field_73,/*Fog*/
                    field_10 = class_238.field_1989.field_73 /*Base 2*/
                },
                field_2296 = true,
                QuintAtomType = "Phlogiston:phlogiston"
            };

            QApi.AddAtomType(Phlogiston);
        }
    }
}
