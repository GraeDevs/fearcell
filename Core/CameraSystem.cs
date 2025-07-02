using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace fearcell.Core
{
    public class CameraSystem : ModSystem
    {
        public static int ShakeTimer = 0;
        public static float ShakeAmount = 0;
        public override void ModifyScreenPosition()
        {
            Player player = Main.LocalPlayer;

            if (!isChangingCameraPos && !isChangingZoom)
            {
                zoomBefore = Main.GameZoomTarget;
            }
            if (isChangingZoom)
            {
                if (--zoomChangeLength > 0)
                {
                    if (zoomAmount != 1 && zoomAmount > zoomBefore)
                    {
                        Main.GameZoomTarget = Utils.Clamp(Main.GameZoomTarget + 0.05f, 1f, zoomAmount);
                    }
                    if (zoomChangeTransition <= 1f)
                    {
                        zoomChangeTransition += 0.025f;
                    }
                }
                else if (zoomChangeTransition >= 0)
                {
                    if (Main.GameZoomTarget > zoomBefore)
                    {
                        Main.GameZoomTarget -= 0.05f;
                    }
                    zoomChangeTransition -= 0.025f;
                }
                else isChangingZoom = false;
            }
            if (isChangingCameraPos)
            {
                if (CameraChangeLength > 0)
                {
                    if (zoomAmount != 1 && zoomAmount > zoomBefore)
                    {
                        Main.GameZoomTarget = Utils.Clamp(Main.GameZoomTarget + 0.05f, 1f, zoomAmount);
                    }
                    if (CameraChangeTransition <= 1f)
                    {
                        Main.screenPosition = Vector2.SmoothStep(cameraChangeStartPoint, CameraChangePos, CameraChangeTransition += 0.025f);
                    }
                    else
                    {
                        Main.screenPosition = CameraChangePos;
                    }
                    CameraChangeLength--;
                }
                else if (CameraChangeTransition >= 0)
                {
                    if (Main.GameZoomTarget > zoomBefore)
                    {
                        Main.GameZoomTarget -= 0.05f;
                    }
                    Main.screenPosition = Vector2.SmoothStep(player.Center - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), CameraChangePos, CameraChangeTransition -= 0.05f);
                }
                else
                {
                    isChangingCameraPos = false;
                }
            }
        }
        float zoomBefore;
        public static float zoomAmount;
        public static Vector2 cameraChangeStartPoint;
        public static Vector2 CameraChangePos;
        public static float CameraChangeTransition, zoomChangeTransition;
        public static int CameraChangeLength, zoomChangeLength;
        public static bool isChangingCameraPos, isChangingZoom;
        public static void ChangeZoom(float zoom, int len)
        {
            zoomAmount = zoom;
            zoomChangeLength = len;
            isChangingZoom = true;
            zoomChangeTransition = 0;
        }
        public static void ChangeCameraPos(Vector2 pos, int length, float zoom = 1.65f)
        {
            cameraChangeStartPoint = Main.screenPosition;
            CameraChangeLength = length;
            CameraChangePos = pos - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
            isChangingCameraPos = true;
            CameraChangeTransition = 0;
            if (Main.GameZoomTarget < zoom)
                zoomAmount = zoom;
        }
    }
}
