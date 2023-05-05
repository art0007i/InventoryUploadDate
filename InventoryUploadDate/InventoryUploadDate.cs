using HarmonyLib;
using NeosModLoader;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using FrooxEngine;
using FrooxEngine.UIX;
using BaseX;

namespace InventoryUploadDate
{
    public class InventoryUploadDate : NeosMod
    {
        public override string Name => "InventoryUploadDate";
        public override string Author => "art0007i";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/art0007i/InventoryUploadDate/";
        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("me.art0007i.InventoryUploadDate");
            harmony.PatchAll();

        }

        public const string DATE_FORMAT_STR = "Uploaded on: {0}";
        public const string DATE_TEXT_NAME = "DateCreatedText";

        [HarmonyPatch(typeof(InventoryBrowser), "OnAttach")]
        class InventoryUploadDatePatch
        {
            public static void Postfix(SyncRef<Slot> ____buttonsRoot)
            {
                var tx = ____buttonsRoot.Target.Parent.AddSlot(DATE_TEXT_NAME);
                tx.AttachComponent<RectTransform>().AnchorMax.Value = new float2(0.6f, 1);
                var txt = tx.AttachComponent<Text>();
                txt.Content.Value = string.Format(DATE_FORMAT_STR, "---");
                txt.AutoSizeMax.Value = 32f;
                txt.HorizontalAutoSize.Value = true;
                txt.VerticalAutoSize.Value = true;
                txt.Color.Value = color.Gray;
                txt.Align = Alignment.MiddleCenter;
            }
        }

        [HarmonyPatch(typeof(InventoryBrowser), "OnChanges")]
        class InventoryItemSelectionPatch
        {
            public static void Postfix(InventoryBrowser __instance, SyncRef<Slot> ____buttonsRoot)
            {
                InventoryItemUI inventoryItemUI = __instance.SelectedInventoryItem;
                var dateString = "---";
                if (inventoryItemUI != null)
                {
                    Record r = (Traverse.Create(inventoryItemUI).Field("Item").GetValue() as Record)
                        ?? (Traverse.Create(inventoryItemUI).Field("Directory").GetValue() as RecordDirectory).EntryRecord;
                    if(r != null)
                    {
                        dateString = r.LastModificationTime.ToString();
                    }
                }
                var sl = ____buttonsRoot.Target.Parent.Find(DATE_TEXT_NAME);
                if (sl == null) return;
                sl.GetComponent<Text>().Content.Value = string.Format(DATE_FORMAT_STR, dateString);
            }
        }
    }
}