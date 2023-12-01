﻿using MonoMod.RuntimeDetour;
using Quintessential;
using System.Collections.Generic;
using System.Reflection;

using PartTypes = class_191;
using AtomTypes = class_175;
using Texture = class_256;
using System.Security.Policy;

namespace Phlogiston
{
    public class PhlogistonMod : QuintessentialMod
    {
        //define a convenient helper
        public static MethodInfo PrivateMethod<T>(string method) => typeof(T).GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        //stuff we need to hook into a private method
        private static IDetour hook_Sim_method_1832;
        private delegate void orig_Sim_method_1832(Sim self, bool isConsumptionHalfstep);

        public override void Load()
        {
            
        }

        public override void PostLoad()
        {
            
        }

        public override void Unload()
        {
            hook_Sim_method_1832.Dispose();
        }

        public override void LoadPuzzleContent()
        {
            // hook_Sim_method_1832 = new Hook(PrivateMethod<Sim>("method_1832"), PrivateMethod<PhlogistonMod>("GlyphFiringCode"));
            PhlogistonAtoms.AddAtomTypes();
            PhlogistonGlyphs.AddGlyphs();
            QApi.RunAfterCycle(CustomGlyphCode);
        }

        // the actual code that does the thing
        private static void GlyphFiringCode(orig_Sim_method_1832 orig, Sim sim_self, bool isConsumptionHalfstep)
        {
            PhlogistonAtoms.Phlogiston.field_2293 = false;
            orig(sim_self, isConsumptionHalfstep);
            PhlogistonAtoms.Phlogiston.field_2293 = true;
        }

        private static void CustomGlyphCode(Sim sim_self, bool isConsumptionHalfstep)
        {
            var SEB = sim_self.field_3818;
            var solution = SEB.method_502();
            var partList = solution.field_3919;
            var partSimStates = sim_self.field_3821;
            var struct122List = sim_self.field_3826;
            var moleculeList = sim_self.field_3823;
            var gripperList = sim_self.HeldGrippers;

            //define some helpers
            Maybe<AtomReference> maybeFindAtom(Part part, HexIndex hex, List<Part> list, bool checkWheels = false)
            {
                return (Maybe<AtomReference>)PrivateMethod<Sim>("method_1850").Invoke(sim_self, new object[] { part, hex, list, checkWheels });
            }

            void spawnAtomAtHex(Part part, HexIndex hex, AtomType atom)
            {
                Molecule molecule = new Molecule();
                molecule.method_1105(new Atom(atom), part.method_1184(hex));
                moleculeList.Add(molecule);
            }

            void consumeAtomReference(AtomReference atomRef)
            {
                // delete the input atom
                atomRef.field_2277.method_1107(atomRef.field_2278);
                // draw input getting consumed
                SEB.field_3937.Add(new class_286(SEB, atomRef.field_2278, atomRef.field_2280));
            }

            bool doesBondExist(Part part, Molecule molecule, enum_126 bond, HexIndex index1, HexIndex index2)
            {
                var index1True = part.method_1184(index1);
                var index2True = part.method_1184(index2);

                return molecule.method_1111(bond, index1True, index2True);
            }

            // var GlyphProjection = PartTypes.field_1778;
            foreach (Part part in partList) {
                PartSimState partSimState = partSimStates[part];
                var partType = part.method_1159();

                if (partType == PhlogistonGlyphs.Combustion)
                {
                    var inputL = new HexIndex(0, 0);
                    var inputR = new HexIndex(1, 0);
                    var outputL = new HexIndex(0, -1);
                    var outputR = new HexIndex(2, -1);

                    var leftAtom = default(AtomReference);
                    var rightAtom = default(AtomReference);

                    // Check if both inputs have an atom
                    if (maybeFindAtom(part, inputL, partList).method_99(out leftAtom) && maybeFindAtom(part, inputR, partList).method_99(out rightAtom))
                    {
                        // Check if both atoms are fire and unheld.
                        if (
                            leftAtom.field_2280 == AtomTypes.field_1678 // fire
                            && rightAtom.field_2280 == AtomTypes.field_1678 // fire
                            && !leftAtom.field_2282 // held by gripper
                            && !rightAtom.field_2282 // held by gripper
                            )
                        {
                            List<Molecule> molecules = sim_self.field_3823;
                            Molecule inputMolecule = null;

                            foreach (Molecule molecule in molecules)
                            {
                                var none = enum_126.Prisma0;
                                var prisma0 = enum_126.Prisma0;
                                var prisma1 = enum_126.Prisma1;
                                var prisma2 = enum_126.Prisma2;

                                HexIndex[] noBondIndiciesL = new HexIndex[]
                                {
                                    new HexIndex(1, -1),
                                    new HexIndex(0, -1),
                                    new HexIndex(-1, 0),
                                    new HexIndex(0, 1),
                                    new HexIndex(1, 1),
                                    new HexIndex(2, -1),
                                    new HexIndex(2, 0),
                                    new HexIndex(2, 1),
                                };
                                bool hasOuterBond = false;

                                // Check if there are no other bonds except between the two fire atoms.
                                foreach (HexIndex index in noBondIndiciesL)
                                {
                                    // If there is a bond, then it's not a pair.
                                    if (doesBondExist(part, molecule, enum_126.None, inputL, index) || doesBondExist(part, molecule, enum_126.None, inputR, index))
                                    {
                                        hasOuterBond = true;
                                        break;
                                    }
                                }

                                if (!hasOuterBond 
                                    && doesBondExist(part, molecule, enum_126.Prisma0, inputL, inputR)
                                    && doesBondExist(part, molecule, enum_126.Prisma1, inputL, inputR)
                                    && doesBondExist(part, molecule, enum_126.Prisma2, inputL, inputR)
                                    )
                                {
                                    // Break Triplix Bond
                                    molecule.method_1112(enum_126.None, part.method_1184(inputL), part.method_1184(inputR), struct_18.field_1431);

                                    // Consume atoms
                                    consumeAtomReference(leftAtom);
                                    consumeAtomReference(rightAtom);

                                    // Finally, spawn Phlogiston atoms
                                    spawnAtomAtHex(part, outputL, PhlogistonAtoms.Phlogiston);
                                    spawnAtomAtHex(part, outputR, PhlogistonAtoms.Phlogiston);

                                    // No need to iterate over the rest of the molecules
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        public static Vector2 hexGraphicalOffset(HexIndex hex) => class_187.field_1742.method_492(hex);
    }
}