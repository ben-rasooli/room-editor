using Unity.Mathematics;
using Unity.Tiny;

namespace Project
{
  public static partial class ExtensionMethods
  {
    public static float4 ToFloat4(this Color c)
    {
      return new float4(c.r, c.g, c.b, c.a);
    }

    public static string ToString(this PanelType panelType, int dummy)
    {
      switch (panelType)
      {
        case PanelType.Thin_220:
          return "Thin_220 ";
        case PanelType.Thin_230:
          return "Thin_230 ";
        case PanelType.Thin_240:
          return "Thin_240 ";
        case PanelType.Thin_260:
          return "Thin_260 ";
        case PanelType.Thin_280:
          return "Thin_280 ";
        case PanelType.Thin_300:
          return "Thin_300 ";
        case PanelType.Thin_360:
          return "Thin_360 ";
        case PanelType.Thin_420:
          return "Thin_420 ";
        case PanelType.Thin_460:
          return "Thin_460 ";
        case PanelType.Thin_480:
          return "Thin_480 ";
        case PanelType.Thin_600:
          return "Thin_600 ";
        case PanelType.Thick_240:
          return "Thick_240";
        case PanelType.Thick_270:
          return "Thick_270";
        case PanelType.Thick_370:
          return "Thick_370";
        case PanelType.Thick_460:
          return "Thick_460";
        case PanelType.Door:
          return "Door     ";
        default:
          return "????     ";
      }
    }

    public static string ToString(this PanelType panelType, bool dummy)
    {
      switch (panelType)
      {
        case PanelType.Thin_220:
          return "Thin_220";
        case PanelType.Thin_230:
          return "Thin_230";
        case PanelType.Thin_240:
          return "Thin_240";
        case PanelType.Thin_260:
          return "Thin_260";
        case PanelType.Thin_280:
          return "Thin_280";
        case PanelType.Thin_300:
          return "Thin_300";
        case PanelType.Thin_360:
          return "Thin_360";
        case PanelType.Thin_420:
          return "Thin_420";
        case PanelType.Thin_460:
          return "Thin_460";
        case PanelType.Thin_480:
          return "Thin_480";
        case PanelType.Thin_600:
          return "Thin_600";
        case PanelType.Thick_240:
          return "Thick_240";
        case PanelType.Thick_270:
          return "Thick_270";
        case PanelType.Thick_370:
          return "Thick_370";
        case PanelType.Thick_460:
          return "Thick_460";
        case PanelType.Door:
          return "Door";
        default:
          return "";
      }
    }

    public static bool RoughlyEquals(this float4 lhs, float4 rhs)
    {
      return
        math.abs(lhs.x - rhs.x) < 0.0001f &&
        math.abs(lhs.y - rhs.y) < 0.0001f &&
        math.abs(lhs.z - rhs.z) < 0.0001f &&
        math.abs(lhs.w - rhs.w) < 0.0001f;
    }
  }
}
