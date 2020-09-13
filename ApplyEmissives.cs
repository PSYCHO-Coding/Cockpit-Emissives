using Sandbox.ModAPI;
using System.Linq;
using System.Threading.Tasks;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.Entities;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Interfaces.Terminal;
using SpaceEngineers.Game.ModAPI;
using System;
using System.Collections.Generic;
using System.Text;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRageMath;
using VRage.Utils;

namespace PSYCHO.ApplyEmissives
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_Cockpit), false, "HK_Cockpit_Small", "HK_Cockpit_Small2")]

    public class ApplyEmissivesLogic : MyGameLogicComponent
    {
        //private int Tick;
        IMyCockpit Block;
        Vector3 BlockColor = Vector3.Zero;
        MyStringHash BlockTexture;

        // USER CHANGABLE VARIABLES
        Color FullyWorkiongEmissiveColor = new Color(242, 110, 80);
        Color BustedEmissiveColor = new Color(0, 0, 0);
        Color EmissiveColor = new Color(0, 0, 0);

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            Block = Entity as IMyCockpit;

            NeedsUpdate = MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
        }

        public override void UpdateOnceBeforeFrame()
        {
            if (Block == null)
                return;

            // Hook to events.
            Block.PropertiesChanged += PropertiesChanged;
            Block.IsWorkingChanged += IsWorkingChanged;

            NeedsUpdate = MyEntityUpdateEnum.EACH_100TH_FRAME;
        }

        private void PropertiesChanged(IMyTerminalBlock obj)
        {
            BlockColor = Block.SlimBlock.ColorMaskHSV;
            BlockTexture = Block.SlimBlock.SkinSubtypeId;

            CheckAndSetEmissives();
        }

        private void IsWorkingChanged(IMyCubeBlock obj)
        {
            BlockColor = Block.SlimBlock.ColorMaskHSV;
            BlockTexture = Block.SlimBlock.SkinSubtypeId;

            CheckAndSetEmissives();
        }

        // Cleanup.
        public override void Close()
        {
            Block.PropertiesChanged -= PropertiesChanged;
            Block.IsWorkingChanged -= IsWorkingChanged;

            Block = null;
        }

        // Mostly needed due to not having a 'block recolored' event.
        public override void UpdateBeforeSimulation100()
        {
            if (Block == null)
            {
                NeedsUpdate = MyEntityUpdateEnum.NONE;
                return;
            }

            // Only if we want to use it within some frame set not available natively.
            // In this instance, it's cosmetic in nature so we don't care if the effects apply at a delay so we're handling it in a update 100 (each 100th frame).
            /*
            Tick++;

            if (Tick % 3 == 0)
            {
                // Do conditionals here...
            }
            */

            bool updateEmissives = false;

            if (BlockColor != Block.SlimBlock.ColorMaskHSV)
            {
                BlockColor = Block.SlimBlock.ColorMaskHSV;
                updateEmissives = true;
            }

            if (BlockTexture != Block.SlimBlock.SkinSubtypeId)
            {
                BlockTexture = Block.SlimBlock.SkinSubtypeId;
                updateEmissives = true;
            }

            if (updateEmissives)
            {
                CheckAndSetEmissives();
            }
        }

        // Conditions and colors go here.
        void CheckAndSetEmissives()
        {
            if (Block.IsFunctional)
            {
                if (Block.IsWorking)
                {
                    if (EmissiveColor != FullyWorkiongEmissiveColor)
                    {
                        EmissiveColor = FullyWorkiongEmissiveColor;
                        Block.SetEmissiveParts("Emissive2", EmissiveColor, 10f);
                    }
                }
                else
                {
                    if (EmissiveColor != BustedEmissiveColor)
                    {
                        EmissiveColor = BustedEmissiveColor;
                        Block.SetEmissiveParts("Emissive2", EmissiveColor, 0f);
                    }
                }
            }
            else
            {
                if (EmissiveColor != BustedEmissiveColor)
                {
                    EmissiveColor = BustedEmissiveColor;
                    Block.SetEmissiveParts("Emissive2", EmissiveColor, 0f);
                }
            }
        }
    }
}
