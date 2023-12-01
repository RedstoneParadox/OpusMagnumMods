using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Quintessential;

using PartType = class_139;
using PartTypes = class_191;
using Texture = class_256;

namespace Phlogiston
{
    internal class PhlogistonGlyphs
    {
        // Part Types
        public static PartType Combustion;
        // Puzzle Options
        public static PuzzleOption CombustionOption;
        public static void AddGlyphs()
        {
            string path, iconpath, selectpath;
            path = "phlogiston/textures/";
            iconpath = path + "parts/icons/";
            selectpath = path + "select/";

            Combustion = makeGlyph(
                "phlogiston-combustion",
                "Glyph of Combustion",
                "the glyph of combustion forces triplix-bonded fire to combust, transmuting it to phlogiston",
                30,
                new HexIndex[4] { new HexIndex(0, 0), new HexIndex(0, -1), new HexIndex(1, 0), new HexIndex(2, -1) },
                class_235.method_615(iconpath + "combustion"),
                class_235.method_615(iconpath + "combustion_hover"),
                class_238.field_1989.field_97.field_374, // double_glow
                class_238.field_1989.field_97.field_375, // double_stroke
                false
            );

            var combustionPath = path + "parts/combustion/";
            Texture combustionBase = class_235.method_615(combustionPath + "combustion_base");
            Texture pipeRing = class_235.method_615("textures/parts/pipe_ring");
            Texture combustionBond = class_235.method_615(combustionPath + "combustion_bond");

            QApi.AddPartType(Combustion, (part, pos, editor, renderer) => {
                PartSimState partSimState = editor.method_507().method_481(part);
                var simTime = editor.method_504();

                var originHex = new HexIndex(0, 0);

                float partAngle = renderer.field_1798;
                Vector2 base_offset = new Vector2(90f, 180f);
                drawPartGraphic(renderer, combustionBase, base_offset, 0.0f, Vector2.Zero, new Vector2(-1f, -1f));

                var leftInputHex = originHex;
                var rightInputHex = new HexIndex(1, 0);

                drawPartGraphic(renderer, pipeRing, new Vector2(41f, 48f), 0f, Vector2.Zero, new Vector2(-1f, -1f));
                drawPartGraphic(renderer, pipeRing, new Vector2(-41f, 48f), 0f, Vector2.Zero, new Vector2(-1f, -1f));
                drawPartGraphic(renderer, combustionBond, new Vector2(-25f, 23f), 0f, Vector2.Zero, new Vector2(-1f, -1f));
            });
            QApi.AddPartTypeToPanel(Combustion, false);
        }

        // Theft
        private static PartType makeGlyph(
        string id,
        string name,
        string desc,
        int cost,
        HexIndex[] footprint,
        Texture icon,
        Texture hover,
        Texture glow,
        Texture stroke,
        bool onlyOne = false)
        {
            QApi.AddPuzzlePermission(id, name);
            PartType ret = new PartType()
            {
                /*ID*/
                field_1528 = id,
                /*Name*/
                field_1529 = class_134.method_253(name, string.Empty),
                /*Desc*/
                field_1530 = class_134.method_253(desc, string.Empty),
                /*Cost*/
                field_1531 = cost,
                /*Is a Glyph?*/
                field_1539 = true,
                /*Hex Footprint*/
                field_1540 = footprint,
                /*Icon*/
                field_1547 = icon,
                /*Hover Icon*/
                field_1548 = hover,
                /*Glow (Shadow)*/
                field_1549 = glow,
                /*Stroke (Outline)*/
                field_1550 = stroke,
                /*Only One Allowed?*/
                field_1552 = onlyOne,
                CustomPermissionCheck = perms => true
            };
            return ret;
        }

        public static void DrawSelectorFlash(SolutionEditorBase SEB, Part part, HexIndex hex)
        {
            //SEB.field_3935.Add(new class_228(SEB, (enum_7)1, PhlogistonMod.hexGraphicalOffset(hex.Rotated(part.method_1163()) + part.method_1161()), ProliferationFlashAnimation, 30f, Vector2.Zero, 0f));
        }

        #region drawingHelpers
        private static Vector2 hexGraphicalOffset(HexIndex hex) => PhlogistonMod.hexGraphicalOffset(hex);
        private static Vector2 textureDimensions(Texture tex) => tex.field_2056.ToVector2();
        private static Vector2 textureCenter(Texture tex) => (textureDimensions(tex) / 2).Rounded();
        private static void drawPartGraphic(class_195 renderer, Texture tex, Vector2 graphicPivot, float graphicAngle, Vector2 graphicTranslation, Vector2 screenTranslation)
        {
            drawPartGraphicScaled(renderer, tex, graphicPivot, graphicAngle, graphicTranslation, screenTranslation, new Vector2(1f, 1f));
        }

        private static void drawPartGraphicScaled(class_195 renderer, Texture tex, Vector2 graphicPivot, float graphicAngle, Vector2 graphicTranslation, Vector2 screenTranslation, Vector2 scaling)
        {
            //for graphicPivot and graphicTranslation, rightwards is the positive-x direction and upwards is the positive-y direction
            //graphicPivot is an absolute position, with (0,0) denoting the bottom-left corner of the texture
            //graphicTranslation is a translation, so (5,-3) means "translate 5 pixels right and 3 pixels down"
            //graphicAngle is measured in radians, and counterclockwise is the positive-angle direction
            //screenTranslation is the final translation applied, so it is not affected by rotations
            Matrix4 matrixScreenPosition = Matrix4.method_1070(renderer.field_1797.ToVector3(0f));
            Matrix4 matrixTranslateOnScreen = Matrix4.method_1070(screenTranslation.ToVector3(0f));
            Matrix4 matrixRotatePart = Matrix4.method_1073(renderer.field_1798);
            Matrix4 matrixTranslateGraphic = Matrix4.method_1070(graphicTranslation.ToVector3(0f));
            Matrix4 matrixRotateGraphic = Matrix4.method_1073(graphicAngle);
            Matrix4 matrixPivotOffset = Matrix4.method_1070(-graphicPivot.ToVector3(0f));
            Matrix4 matrixScaling = Matrix4.method_1074(scaling.ToVector3(0f));
            Matrix4 matrixTextureSize = Matrix4.method_1074(tex.field_2056.ToVector3(0f));

            Matrix4 matrix4 = matrixScreenPosition * matrixTranslateOnScreen * matrixRotatePart * matrixTranslateGraphic * matrixRotateGraphic * matrixPivotOffset * matrixScaling * matrixTextureSize;
            class_135.method_262(tex, Color.White, matrix4);
        }

        private static void drawPartGraphicSpecular(class_195 renderer, Texture tex, Vector2 graphicPivot, float graphicAngle, Vector2 graphicTranslation, Vector2 screenTranslation)
        {
            float specularAngle = (renderer.field_1799 - (renderer.field_1797 + graphicTranslation.Rotated(renderer.field_1798))).Angle() - 1.570796f - renderer.field_1798;
            drawPartGraphic(renderer, tex, graphicPivot, graphicAngle + specularAngle, graphicTranslation, screenTranslation);
        }

        private static void drawPartGloss(class_195 renderer, Texture gloss, Texture glossMask, Vector2 offset)
        {
            drawPartGloss(renderer, gloss, glossMask, offset, new HexIndex(0, 0), 0f);
        }
        private static void drawPartGloss(class_195 renderer, Texture gloss, Texture glossMask, Vector2 offset, HexIndex hexOffset, float angle)
        {
            class_135.method_257().field_1692 = class_238.field_1995.field_1757; // MaskedGlossPS shader
            class_135.method_257().field_1693[1] = gloss;
            var hex = new HexIndex(0, 0);
            Vector2 method2001 = 0.0001f * (renderer.field_1797 + hexGraphicalOffset(hex).Rotated(renderer.field_1798) - 0.5f * class_115.field_1433);
            class_135.method_257().field_1695 = method2001;
            drawPartGraphic(renderer, glossMask, offset, angle, hexGraphicalOffset(hexOffset), Vector2.Zero);
            class_135.method_257().field_1692 = class_135.method_257().field_1696; // previous shader
            class_135.method_257().field_1693[1] = class_238.field_1989.field_71;
            class_135.method_257().field_1695 = Vector2.Zero;
        }
        private static void drawAtomIO(class_195 renderer, AtomType atomType, HexIndex hex, float num)
        {
            Molecule molecule = Molecule.method_1121(atomType);
            Vector2 method1999 = renderer.field_1797 + hexGraphicalOffset(hex).Rotated(renderer.field_1798);
            Editor.method_925(molecule, method1999, new HexIndex(0, 0), 0f, 1f, num, 1f, false, null);
        }
        #endregion
    }
}
