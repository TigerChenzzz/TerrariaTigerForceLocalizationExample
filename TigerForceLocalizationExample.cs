using MonoMod.Cil;
using System.Reflection;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using TigerForceLocalizationLib;
using TigerForceLocalizationLib.Filters;
using TypeFilter = TigerForceLocalizationLib.Filters.TypeFilter;

namespace TigerForceLocalizationExample;

public class TigerForceLocalizationExample : Mod {
    // ↓↓↓将此值设置为 true 并且清空 .hjson 文件中的内容 (不要删除 .hjson 文件) 以进行初次构建
    private readonly static bool firstRegister = false;
    public override void PostSetupContent() {
        // 这里作为示例就直接把自己作为本地化的目标模组了, 实际上应该是需要本地化的模组
        string targetModName = "TigerForceLocalizationExample";

        // 对于弱引用需先判断模组是否已加载
        if (!ModLoader.HasMod(targetModName))
            return;

        if (firstRegister) {
            TigerForceLocalizationHelper.LocalizeAll(nameof(TigerForceLocalizationExample), targetModName, true, filters: new() {
                TypeFilter = TypeFilter.MismatchFullName(typeof(TigerForceLocalizationExample).FullName) 
                & new TypeFilter(type => !type.FullName.StartsWith(typeof(TigerForceLocalizationExample).FullName + '+')), // 筛掉此类和此类的内嵌类
                MethodFilter = MethodFilter.CommonMethodFilter,
                CursorFilter = ILCursorFilter.CommonCursorFilter,
            });
            TigerForceLocalizationHelper.ShowLocalizationRegisterProgress(); // 这一句放在上面或者下面或者 Load 或者 OnModLoad 等地方都可以, 只要是加载阶段就好
            return;
        }

        // 以下任意方法均可使用
        switch (Main.rand.Next(6)) {
        case 0:
            LocalizeMethod_LocalizeAll(targetModName);
            break;
        case 1:
            LocalizeMethod_LocalizeMethod(targetModName);
            break;
        case 2:
            LocalizeMethod_LocalizeMethodByRoot(targetModName);
            break;
        case 3:
            LocalizeMethod_ForceLocalizeSystemImpl();
            break;
        case 4:
            LocalizeMethod_ForceLocalizeSystemByLocalizedTextImpl();
            break;
        case 5:
            LocalizeMethod_ForceLocalizeSystem();
            break;
        }
    }
    public static void LocalizeMethod_LocalizeAll(string targetModName) {
        TigerForceLocalizationHelper.LocalizeAll(nameof(TigerForceLocalizationExample), targetModName, registerKey: false);
    }
    public static void LocalizeMethod_LocalizeMethod(string targetModName) {
        var mod = ModLoader.GetMod(targetModName);
        var typeFullName = "TigerForceLocalizationExample.Content.Items.ExampleSword";
        var type = mod.Code.GetType(typeFullName);
        if (type == null)
            return;
        var method = type.GetMethod("UseAnimation");
        if (method != null)
            TigerForceLocalizationHelper.LocalizeMethod(method, $"Mods.TigerForceLocalizationExample.ForceLocalizations.{typeFullName}.UseAnimation");
        method = type.GetMethod("AltUseAnimation", BindingFlags.NonPublic | BindingFlags.Instance);
        TigerForceLocalizationHelper.LocalizeMethod(method, $"Mods.TigerForceLocalizationExample.ForceLocalizations.{typeFullName}.AltUseAnimation");
    }
    public static void LocalizeMethod_LocalizeMethodByRoot(string targetModName) {
        var mod = ModLoader.GetMod(targetModName);
        var typeFullName = "TigerForceLocalizationExample.Content.Items.ExampleSword";
        var root = "Mods.TigerForceLocalizationExample.ForceLocalizations";
        var type = mod.Code.GetType(typeFullName);
        if (type == null)
            return;
        var method = type.GetMethod("UseAnimation");
        if (method != null)
            TigerForceLocalizationHelper.LocalizeMethodByRoot(method, root);
        method = type.GetMethod("AltUseAnimation", BindingFlags.NonPublic | BindingFlags.Instance);
        if (method != null)
            TigerForceLocalizationHelper.LocalizeMethodByRoot(method, root);
    }
    /// <summary>
    /// 直接使用 <see cref="ForceLocalizeSystemImpl{TMod}"/>
    /// 需要注意弱引用时的处理
    /// </summary>
    public static void LocalizeMethod_ForceLocalizeSystemImpl() {
        var typeFullName = "TigerForceLocalizationExample.Content.Items.ExampleSword";
        ForceLocalizeSystemImpl<TigerForceLocalizationExample>.LocalizeByTypeFullName(typeFullName, "AltUseAnimation", new() {
            { "use type has changed to ", "使用方式已被更改为" },
        });
        // 存在相同的片段被翻译为不同的片段, 需要使用带有 InOrder 的方法
        ForceLocalizeSystemImpl<TigerForceLocalizationExample>.LocalizeInOrderByTypeFullName(typeFullName, "UseAnimation", [
            ("get an item of id ", "获取了一个 id 为"),
            ("!"                 , "的物品!"),
            ("get a buff of id " , "获取了一个 id 为"),
            ("!"                 , "的 Buff!"),
            ("Wrong use type!"   , "使用方式有误!"),
        ]);
    }
    public static void LocalizeMethod_ForceLocalizeSystemByLocalizedTextImpl() {
        var typeKey = "Mods.TigerForceLocalizationExample.ForceLocalizations.TigerForceLocalizationExample.Content.Items.ExampleSword";
        var typeFullName = "TigerForceLocalizationExample.Content.Items.ExampleSword";
        ForceLocalizeSystemByLocalizeTextImpl<TigerForceLocalizationExample>.LocalizeByTypeFullName(typeFullName, "AltUseAnimation", new() {
            { "use type has changed to ", typeKey + ".AltUseAnimation.1.NewString" },
        });
        // 存在相同的片段被翻译为不同的片段, 需要使用带有 InOrder 的方法
        ForceLocalizeSystemByLocalizeTextImpl<TigerForceLocalizationExample>.LocalizeInOrderByTypeFullName(typeFullName, "UseAnimation", [
            ("get an item of id ", typeKey + ".UseAnimation.1.NewString"),
            ("!"                 , typeKey + ".UseAnimation.2.NewString_1"),
            ("get a buff of id " , typeKey + ".UseAnimation.3.NewString"),
            ("!"                 , typeKey + ".UseAnimation.2.NewString_2"),
            ("Wrong use type!"   , typeKey + ".UseAnimation.4.NewString"),
        ]);
    }
    #region 继承 ForceLocalizeSystem 并使用
    public static void LocalizeMethod_ForceLocalizeSystem() {
        var typeFullName = "TigerForceLocalizationExample.Content.Items.ExampleSword";
        MyLocalizeSystem.LocalizeByTypeFullName(typeFullName, "AltUseAnimation", new() {
            { "use type has changed to ", typeFullName + ".AltUseAnimation.1.NewString" },
        });
        // 存在相同的片段被翻译为不同的片段, 需要使用带有 InOrder 的方法
        MyLocalizeSystem.LocalizeInOrderByTypeFullName(typeFullName, "UseAnimation", [
            ("get an item of id ", typeFullName + ".UseAnimation.1.NewString"),
            ("!"                 , typeFullName + ".UseAnimation.2.NewString_1"),
            ("get a buff of id " , typeFullName + ".UseAnimation.3.NewString"),
            ("!"                 , typeFullName + ".UseAnimation.2.NewString_2"),
            ("Wrong use type!"   , typeFullName + ".UseAnimation.4.NewString"),
        ]);
    }

    // [JITWhenModsEnabled("TigerForceLocalizationExample")] // <- 对于弱引用则最好加上
    public class MyLocalizeSystem : ForceLocalizeSystem<MyLocalizeSystem> {
        protected override string ModName => "TigerForceLocalizationExample";
        /*
        // 这么写也可以支持 hjson 的键, 但是没法支持 hjson 热重载和语言切换, 需要重写 ReplaceString_IL
        protected override string ReplaceString(string old, string @new) {
            @new = "Mods.TigerForceLocalizationExample.ForceLocalizations." + @new;
            return Language.Exists(@new) ? Language.GetTextValue(@new) : old;
        }
        */
        protected override void ReplaceString_IL(ILCursor cursor, string old, string @new) {
            @new = "Mods.TigerForceLocalizationExample.ForceLocalizations." + @new;
            if (!Language.Exists(@new)) {
                return;
            }
            cursor.MoveAfterLabels();
            // 用 Language.GetTextValue(@new) 替换 old
            cursor.EmitLdstr(@new);
            cursor.EmitCall(languageGetTextValueMethod);
            cursor.Remove();
        }
        private readonly MethodInfo languageGetTextValueMethod = typeof(Language).GetMethod(nameof(Language.GetTextValue), BindingFlags.Static | BindingFlags.Public, [typeof(string)])!;
    }
    #endregion
}
