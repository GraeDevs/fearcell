using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using System.Collections.ObjectModel;
using Terraria.UI.Chat;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using ReLogic.Graphics;
using Terraria.Chat;
using Terraria.GameContent.UI.Chat;
using System.IO;
using System.Text;
using static Terraria.ModLoader.ModContent;
using Terraria.Graphics.Shaders;
using System.ComponentModel;
using static tModPorter.ProgressUpdate;
using System.Linq;
using System.Runtime;
using System.Collections.Generic;
using Terraria.UI;
using SubworldLibrary;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ObjectData;


namespace fearcell.Core
{
    public class FearcellSystem : ModSystem
    {
        public static float visualTimer;

        public static int Seed => Main.ActiveWorldFileData.Seed;
        public static string SeedText => Main.ActiveWorldFileData.SeedText;

        public static float ShakeTimer = 0;
        public static float ShakeDuration = 0;
        public static float ShakeIntensity = 0;
        private static Vector2 ShakeOffset = Vector2.Zero;
        private static bool isShaking = false;

        public static bool IsShaking => ShakeTimer > 0f;
        public static float CurrentIntensity => ShakeTimer > 0f ? ShakeIntensity * (ShakeTimer / ShakeDuration) : 0f;
        public static float RemainingTime => ShakeTimer;

        public override void PostUpdateEverything()
        {
           if(!Main.gameMenu)
           {
                // Update shake timer and parameters
                if (ShakeTimer > 0f)
                {
                    // Update timer (60 FPS frame-based timing)
                    ShakeTimer -= 1f / 60f;

                    if (!isShaking)
                    {
                        isShaking = true;
                       // Main.NewText($"Shake started! Duration: {ShakeDuration}", Color.Red);
                    }

                    // Debug every 30 frames (half second)
                    if (Main.GameUpdateCount % 30 == 0)
                    {
                        //Main.NewText($"Shaking: timer={ShakeTimer:F2}, intensity={ShakeIntensity:F1}", Color.White);
                    }
                }
                else if (isShaking)
                {
                    // Shake finished
                    isShaking = false;
                    ShakeOffset = Vector2.Zero;
                    //Main.NewText("Shake ended", Color.Red);
                }
           }
        }

        public override void ModifyScreenPosition()
        {
            if (ShakeTimer > 0f)
            {
                // Calculate shake intensity based on remaining time (fade-out effect)
                float currentIntensity = ShakeIntensity * (ShakeTimer / ShakeDuration);

                // Generate random shake offset
                ShakeOffset = new Vector2(
                    (Main.rand.NextFloat() - 0.5f) * currentIntensity * 2f,
                    (Main.rand.NextFloat() - 0.5f) * currentIntensity * 2f
                );

                // Apply shake offset to screen position
                Main.screenPosition += ShakeOffset;
            }
        }

        public static void StartShake(float intensity, float duration, bool stacking = false)
        {
            if(stacking && ShakeTimer > 0f)
            {
                ShakeIntensity = MathHelper.Max(ShakeIntensity, intensity);
                ShakeDuration = MathHelper.Max(ShakeDuration, duration);
                ShakeTimer = MathHelper.Max(ShakeTimer, duration);
            }
            else
            {
                ShakeIntensity = intensity;
                ShakeDuration = duration;
                ShakeTimer = duration;
            }
        }

        public override void PreUpdateWorld()
        {
            visualTimer += (float)Math.PI / 60;

            if (visualTimer >= Math.PI * 2)
                visualTimer = 0;

            if(SubworldSystem.AnyActive<fearcell>())
            {
                Wiring.UpdateMech();

                TileEntity.UpdateStart();
                foreach (TileEntity tileEntity in TileEntity.ByID.Values)
                {
                    tileEntity.Update();
                }
                TileEntity.UpdateEnd();

                if (++Liquid.skipCount > 1)
                {
                    Liquid.UpdateLiquid();
                    Liquid.skipCount = 0;
                }
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            Player player = Main.LocalPlayer;

            int textIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Cursor")) + 1;

            layers.Insert(textIndex, new LegacyGameInterfaceLayer("Narrator: Text", () =>
            {
                DialogueHandler.DrawDialogue();

                return true;
            }, InterfaceScaleType.UI));

        }
    }

    public static class FearcellUtilities
    {
        public static void Bolt(Vector2 point1, Vector2 point2, int dusttype, float scale = 1, int armLength = 30, Color color = default, float frequency = 0.05f)
        {
            int nodeCount = (int)Vector2.Distance(point1, point2) / armLength;
            Vector2[] nodes = new Vector2[nodeCount + 1];

            nodes[nodeCount] = point2; //adds the end as the last point

            for (int k = 1; k < nodes.Length; k++)
            {
                //Sets all intermediate nodes to their appropriate randomized dot product positions
                nodes[k] = Vector2.Lerp(point1, point2, k / (float)nodeCount) +
                    (k == nodes.Length - 1 ? Vector2.Zero : Vector2.Normalize(point1 - point2).RotatedBy(1.58f) * Main.rand.NextFloat(-armLength / 2, armLength / 2));

                //Spawns the dust between each node
                Vector2 prevPos = k == 1 ? point1 : nodes[k - 1];
                for (float i = 0; i < 1; i += frequency)
                {
                    Dust.NewDustPerfect(Vector2.Lerp(prevPos, nodes[k], i), dusttype, Vector2.Zero, 0, color, scale);
                }
            }
        }
        public static void EasyFurniture(this ModTile tile, int minPick, int width, int height, int dustType, SoundStyle? hitSound, bool tallPadding, Color mapColor, bool solidTop = false, bool solid = false, bool tileLighted = false, string mapName = "", AnchorData bottomAnchor = default, AnchorData topAnchor = default, int[] anchorTiles = null, bool faceDirection = false, bool wallAnchor = false, Point16 Origin = default)
        {
            Main.tileLighted[tile.Type] = tileLighted;
            Main.tileFrameImportant[tile.Type] = true;
            Main.tileLavaDeath[tile.Type] = false;
            Main.tileNoAttach[tile.Type] = true;
            Main.tileSolidTop[tile.Type] = solidTop;
            Main.tileSolid[tile.Type] = solid;
            TileObjectData.newTile.Width = width;
            TileObjectData.newTile.Height = height;
            TileObjectData.newTile.CoordinateHeights = new int[height];

            for (int k = 0; k < height; k++)
            {
                TileObjectData.newTile.CoordinateHeights[k] = 16;
            }

            if (tallPadding)
                TileObjectData.newTile.CoordinateHeights[height - 1] = 18;

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = Origin == default ? new Point16(width / 2, height - 1) : Origin;

            if (bottomAnchor != default)
                TileObjectData.newTile.AnchorBottom = bottomAnchor;

            if (topAnchor != default)
                TileObjectData.newTile.AnchorTop = topAnchor;

            if (anchorTiles != null)
                TileObjectData.newTile.AnchorAlternateTiles = anchorTiles;

            if (wallAnchor)
                TileObjectData.newTile.AnchorWall = true;

            if (faceDirection)
            {
                TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
                TileObjectData.newTile.StyleHorizontal = true;
                TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
                TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
                TileObjectData.addAlternate(1);
            }

            TileObjectData.addTile(tile.Type);
            LocalizedText name = tile.CreateMapEntryName();
            tile.AddMapEntry(mapColor, name);
            tile.DustType = dustType;
            tile.HitSound = hitSound;
            tile.MinPick = minPick;
        }


        public static float RotationDifference(float rotTo, float rotFrom)
        {
            return ((rotTo - rotFrom) % 6.28f + 9.42f) % 6.28f - 3.14f;
        }

        public static float BezierEase(float time)
        {
            return time * time / (2f * (time * time - time) + 1f);
        }

        public static bool CheckCircularCollision(Vector2 center, int radius, Rectangle hitbox)
        {
            if (Vector2.Distance(center, hitbox.TopLeft()) <= radius)
                return true;

            if (Vector2.Distance(center, hitbox.TopRight()) <= radius)
                return true;

            if (Vector2.Distance(center, hitbox.BottomLeft()) <= radius)
                return true;

            return Vector2.Distance(center, hitbox.BottomRight()) <= radius;
        }

        public static Vector3 Vec3(this Vector2 vector)
        {
            return new Vector3(vector.X, vector.Y, 0);
        }

        public static T[] FastUnion<T>(this T[] front, T[] back)
        {
            var combined = new T[front.Length + back.Length];

            Array.Copy(front, combined, front.Length);
            Array.Copy(back, 0, combined, front.Length, back.Length);

            return combined;
        }
       
        public static void Reload(this SpriteBatch spriteBatch, BlendState blendState = default)
        {
            if ((bool)spriteBatch.GetType().GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch))
            {
                spriteBatch.End();
            }
            SpriteSortMode sortMode = SpriteSortMode.Deferred;
            SamplerState samplerState = (SamplerState)spriteBatch.GetType().GetField("samplerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            DepthStencilState depthStencilState = (DepthStencilState)spriteBatch.GetType().GetField("depthStencilState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            RasterizerState rasterizerState = (RasterizerState)spriteBatch.GetType().GetField("rasterizerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            Effect effect = (Effect)spriteBatch.GetType().GetField("customEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            Matrix matrix = (Matrix)spriteBatch.GetType().GetField("transformMatrix", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, matrix);
        }

        public static void Reload(this SpriteBatch spriteBatch, SamplerState _samplerState = default)
        {
            if ((bool)spriteBatch.GetType().GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch))
            {
                spriteBatch.End();
            }
            SpriteSortMode sortMode = SpriteSortMode.Deferred;
            SamplerState samplerState = _samplerState;
            BlendState blendState = (BlendState)spriteBatch.GetType().GetField("blendState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            DepthStencilState depthStencilState = (DepthStencilState)spriteBatch.GetType().GetField("depthStencilState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            RasterizerState rasterizerState = (RasterizerState)spriteBatch.GetType().GetField("rasterizerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            Effect effect = (Effect)spriteBatch.GetType().GetField("customEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            Matrix matrix = (Matrix)spriteBatch.GetType().GetField("transformMatrix", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, matrix);
        }

        public static Texture2D GetAsset(string tex, bool altMethod = false)
        {
            if (altMethod)
                return GetTextureAlt("Assets/" + tex);
            return GetTexture("Assets/" + tex);
        }
        public static Texture2D GetTexture(string path)
        {
            if (path.Contains("fearcell/"))
                return ModContent.Request<Texture2D>(path).Value;
            else
                return ModContent.Request<Texture2D>("fearcell/" + path).Value;
        }
        public static Texture2D GetTextureAlt(string path)
        {
            return fearcell.Instance.Assets.Request<Texture2D>(path).Value;
        }
        public static Color MultiLerpColor(float percent, params Color[] colors)
        {
            float per = 1f / ((float)colors.Length - 1);
            float total = per;
            int currentID = 0;
            while (percent / total > 1f && currentID < colors.Length - 2) { total += per; currentID++; }
            return Color.Lerp(colors[currentID], colors[currentID + 1], (percent - per * currentID) / per);
        }

        public static Vector2 MoveTo(this Vector2 a, Vector2 b, bool normalize = true, bool reverse = false)
        {
            Vector2 baseVel = b - a;
            if (normalize)
                baseVel.Normalize();
            if (reverse)
            {
                Vector2 baseVelReverse = a - b;
                if (normalize)
                    baseVelReverse.Normalize();
                return baseVelReverse;
            }
            return baseVel;
        }

        public static Vector2 TileAdj => (Lighting.Mode == Terraria.Graphics.Light.LightMode.Retro || Lighting.Mode == Terraria.Graphics.Light.LightMode.Trippy) ? Vector2.Zero : Vector2.One * 12;

        public static Vector2 PolarVector(float radius, float theta) =>
          new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * radius;
    }

    public abstract class EaseFunction
    {
        public static readonly EaseFunction Linear = new PolynomialEase((float x) => { return x; });

        public static readonly EaseFunction EaseQuadIn = new PolynomialEase((float x) => { return x * x; });
        public static readonly EaseFunction EaseQuadOut = new PolynomialEase((float x) => { return 1f - EaseQuadIn.Ease(1f - x); });
        public static readonly EaseFunction EaseQuadInOut = new PolynomialEase((float x) => { return (x < 0.5f) ? 2f * x * x : -2f * x * x + 4f * x - 1f; });

        public static readonly EaseFunction EaseCubicIn = new PolynomialEase((float x) => { return x * x * x; });
        public static readonly EaseFunction EaseCubicOut = new PolynomialEase((float x) => { return 1f - EaseCubicIn.Ease(1f - x); });
        public static readonly EaseFunction EaseCubicInOut = new PolynomialEase((float x) => { return (x < 0.5f) ? 4f * x * x * x : 4f * x * x * x - 12f * x * x + 12f * x - 3f; });

        public static readonly EaseFunction EaseQuarticIn = new PolynomialEase((float x) => { return x * x * x * x; });
        public static readonly EaseFunction EaseQuarticOut = new PolynomialEase((float x) => { return 1f - EaseQuarticIn.Ease(1f - x); });
        public static readonly EaseFunction EaseQuarticInOut = new PolynomialEase((float x) => { return (x < 0.5f) ? 8f * x * x * x * x : -8f * x * x * x * x + 32f * x * x * x - 48f * x * x + 32f * x - 7f; });

        public static readonly EaseFunction EaseQuinticIn = new PolynomialEase((float x) => { return x * x * x * x * x; });
        public static readonly EaseFunction EaseQuinticOut = new PolynomialEase((float x) => { return 1f - EaseQuinticIn.Ease(1f - x); });
        public static readonly EaseFunction EaseQuinticInOut = new PolynomialEase((float x) => { return (x < 0.5f) ? 16f * x * x * x * x * x : 16f * x * x * x * x * x - 80f * x * x * x * x + 160f * x * x * x - 160f * x * x + 80f * x - 15f; });

        public static readonly EaseFunction EaseCircularIn = new PolynomialEase((float x) => { return 1f - (float)Math.Sqrt(1.0 - Math.Pow(x, 2)); });
        public static readonly EaseFunction EaseCircularOut = new PolynomialEase((float x) => { return (float)Math.Sqrt(1.0 - Math.Pow(x - 1.0, 2)); });
        public static readonly EaseFunction EaseCircularInOut = new PolynomialEase((float x) => { return (x < 0.5f) ? (1f - (float)Math.Sqrt(1.0 - Math.Pow(x * 2, 2))) * 0.5f : (float)((Math.Sqrt(1.0 - Math.Pow(-2 * x + 2, 2)) + 1) * 0.5); });

        public abstract float Ease(float time);
    }

    public class PolynomialEase : EaseFunction
    {
        private Func<float, float> _function;

        public PolynomialEase(Func<float, float> func)
        {
            _function = func;
        }

        public override float Ease(float time)
        {
            return _function(time);
        }

        //removed because not needed for spirit
        //public static EaseFunction Generate(int factor, int type)
        //{
        //}
    }

    public class EaseBuilder : EaseFunction
    {
        private List<EasePoint> _points;

        public EaseBuilder()
        {
            _points = new List<EasePoint>();
        }

        public void AddPoint(float x, float y, EaseFunction function) => AddPoint(new Vector2(x, y), function);

        public void AddPoint(Vector2 vector, EaseFunction function)
        {
            if (vector.X < 0f) throw new ArgumentException("X value of point is not in valid range!");

            EasePoint newPoint = new EasePoint(vector, function);
            if (_points.Count == 0)
            {
                _points.Add(newPoint);
                return;
            }

            EasePoint last = _points[_points.Count - 1];
            if (last.Point.X > vector.X) throw new ArgumentException("New point has an x value less than the previous point when it should be greater or equal");

            _points.Add(newPoint);
        }

        public override float Ease(float time)
        {
            Vector2 prevPoint = Vector2.Zero;
            EasePoint usePoint = _points[0];
            for (int i = 0; i < _points.Count; i++)
            {
                usePoint = _points[i];
                if (time <= usePoint.Point.X)
                {
                    break;
                }
                prevPoint = usePoint.Point;
            }
            float dist = usePoint.Point.X - prevPoint.X;
            float progress = (time - prevPoint.X) / dist;
            if (progress > 1f) progress = 1f;
            return MathHelper.Lerp(prevPoint.Y, usePoint.Point.Y, usePoint.Function.Ease(progress));
        }

        private struct EasePoint
        {
            public Vector2 Point;
            public EaseFunction Function;

            public EasePoint(Vector2 p, EaseFunction func)
            {
                Point = p;
                Function = func;
            }
        }
    }
}
