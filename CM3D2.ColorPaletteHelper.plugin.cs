
using System;
using System.Reflection;
using UnityEngine;
using UnityInjector;
using UnityInjector.Attributes;

namespace CM3D2.ColorPaletteHelper.Plugin
{
  [PluginFilter("CM3D2x64"), PluginFilter("CM3D2VRx64"), PluginFilter("CM3D2OHx64"), PluginName("CM3D2 Color Palette Helper"), PluginVersion("0.3.1en")]

  public class ColorPaletteHelper : PluginBase
  {
    private class ConstValues
    {
      public static readonly string PLUGIN_NAME = "ColorPaletteHelper";
      public static readonly string PLUGIN_VERSION = "0.3.1en";

      public static readonly int SCENE_EDIT_LEVEL = 5;

      public static readonly int LABEL_FONT_SIZE = 0;
      public static readonly TextAnchor LABEL_ALIGNMENT = TextAnchor.UpperLeft;
      public static readonly Color LABEL_TEXT_COLOR = new Color(0.9f, 0.9f, 0.9f);

      public static readonly int BUTTON_FONT_SIZE = 0;
      public static readonly TextAnchor BUTTON_ALIGNMENT = TextAnchor.MiddleCenter;
      public static readonly Color BUTTON_TEXT_COLOR = new Color(0.9f, 0.9f, 0.9f);

      public static readonly int TOGGLE_FONT_SIZE = 0;
      public static readonly TextAnchor TOGGLE_ALIGNMENT = TextAnchor.UpperLeft;
      public static readonly Color TOGGLE_TEXT_COLOR = new Color(0.891f, 0.891f, 0.891f);

      public struct COLOR_PARAMETER
      {
        public string m_name;
        public int m_min;
        public int m_max;

        public COLOR_PARAMETER(string param_name, int param_min, int param_max)
        {
          m_name = param_name;
          m_min = param_min;
          m_max = param_max;
        }
      }

      public static readonly COLOR_PARAMETER[] COLOR_PARAMETERS = new COLOR_PARAMETER[]{
        new COLOR_PARAMETER("Hue", 0, 255),
        new COLOR_PARAMETER("Saturation", 0, 255),
        new COLOR_PARAMETER("Brightness", 0, 510),
        new COLOR_PARAMETER("Contrast", 0, 200),
        new COLOR_PARAMETER("Hue", 0, 255),
        new COLOR_PARAMETER("Saturation", 0, 255),
        new COLOR_PARAMETER("Brightness", 0, 510),
        new COLOR_PARAMETER("Contrast", 0, 200),
        new COLOR_PARAMETER("Shadow", 0, 255),
      };

      public static readonly string COLOR_INFORMATION_TITLE = "Color Info";
      public static readonly string COLOR_INFORMATION_MAIN_COLOR = "Main";
      public static readonly string COLOR_INFORMATION_SHADOW_COLOR = "Shadow";

      public static readonly string COLOR_EDIT_TITLE = "Edit Color";
      public static readonly string COLOR_EDIT_SETTING_TITLE = "Enabled Tools";
      public static readonly string COLOR_EDIT_SETTING_USE_SLIDER = "Sliders";
      public static readonly string COLOR_EDIT_SETTING_USE_COLOR_SET = "Color Panels";
      public static readonly string COLOR_EDIT_COLOR_TITLE = "Edit Type";
      public static readonly string COLOR_EDIT_COLOR_MAIN = "Main";
      public static readonly string COLOR_EDIT_COLOR_SHADOW = "Shadow";

      public static readonly string COLOR_EDIT_ADJUST_SET_MINUS_1 = "-1";
      public static readonly string COLOR_EDIT_ADJUST_SET_PLUS_1 = "+1";
      public static readonly string COLOR_EDIT_ADJUST_SET_MINUS_10 = "-10";
      public static readonly string COLOR_EDIT_ADJUST_SET_PLUS_10 = "+10";

      public static readonly int COLOR_EDIT_COLOR_SET_BLACK_SQUARE_FONT_SIZE = 40;
      public static readonly string COLOR_EDIT_COLOR_SET_BLACK_SQUARE_CHARACTER = "■";

      public static readonly string RANDOM_EDIT_TITLE = "Random Color";
      public static readonly string RANDOM_EDIT_EXECUTE = "Go";

      public static readonly string UTILITY_EDIT_TITLE = "Utilities";
      public static readonly string UTILITY_EDIT_COPY_MAIN_TO_SHADOW = "Copy main to shadow";
      public static readonly string UTILITY_EDIT_COPY_SHADOW_TO_MAIN = "Copy shadow to main";
      public static readonly string UTILITY_EDIT_SWAP_MAIN_FOR_SHADOW = "Swap colors";
      public static readonly string UTILITY_EDIT_EXECUTE = "Go";

      public static readonly string OTHER_SETTING_TITLE = "Miscellaneous";
      public static readonly string OTHER_SETTING_DISABLE_GAME_CONTROL = "Disable mouse click-through";
    }

    private struct COLOR_VALUE
    {
      public int m_main_hue;
      public int m_main_chroma;
      public int m_main_brightness;
      public int m_main_contrast;
      public int m_shadow_hue;
      public int m_shadow_chroma;
      public int m_shadow_brightness;
      public int m_shadow_contrast;
      public int m_shadow_rate;

      public void clear()
      {
        m_main_hue = 0;
        m_main_chroma = 0;
        m_main_brightness = 0;
        m_main_contrast = 0;
        m_shadow_hue = 0;
        m_shadow_chroma = 0;
        m_shadow_brightness = 0;
        m_shadow_contrast = 0;
        m_shadow_rate = 0;
      }

      public static COLOR_VALUE FromPartsColor(MaidParts.PartsColor param_value)
      {
        COLOR_VALUE retval;
        retval.m_main_hue = param_value.m_nMainHue;
        retval.m_main_chroma = param_value.m_nMainChroma;
        retval.m_main_brightness = param_value.m_nMainBrightness;
        retval.m_main_contrast = param_value.m_nMainContrast;
        retval.m_shadow_hue = param_value.m_nShadowHue;
        retval.m_shadow_chroma = param_value.m_nShadowChroma;
        retval.m_shadow_brightness = param_value.m_nShadowBrightness;
        retval.m_shadow_contrast = param_value.m_nShadowContrast;
        retval.m_shadow_rate = param_value.m_nShadowRate;
        return retval;
      }

      public static COLOR_VALUE FromIntArray(int[] param_value)
      {
        COLOR_VALUE retval;

        if((param_value != null) && (param_value.Length == 9))
        {
          retval.m_main_hue = param_value[0];
          retval.m_main_chroma = param_value[1];
          retval.m_main_brightness = param_value[2];
          retval.m_main_contrast = param_value[3];
          retval.m_shadow_hue = param_value[4];
          retval.m_shadow_chroma = param_value[5];
          retval.m_shadow_brightness = param_value[6];
          retval.m_shadow_contrast = param_value[7];
          retval.m_shadow_rate = param_value[8];
        }
        else
        {
          retval = new COLOR_VALUE();
        }

        return retval;
      }

      public bool equals(COLOR_VALUE param_value)
      {
        return (
          (m_main_hue == param_value.m_main_hue) &&
          (m_main_chroma == param_value.m_main_chroma) &&
          (m_main_brightness == param_value.m_main_brightness) &&
          (m_main_contrast == param_value.m_main_contrast) &&
          (m_shadow_hue == param_value.m_shadow_hue) &&
          (m_shadow_chroma == param_value.m_shadow_chroma) &&
          (m_shadow_brightness == param_value.m_shadow_brightness) &&
          (m_shadow_contrast == param_value.m_shadow_contrast) &&
          (m_shadow_rate == param_value.m_shadow_rate)
        );
      }

      public MaidParts.PartsColor to_parts_color()
      {
        MaidParts.PartsColor retval;
        retval.m_bUse = false;
        retval.m_nMainHue = m_main_hue;
        retval.m_nMainChroma = m_main_chroma;
        retval.m_nMainBrightness = m_main_brightness;
        retval.m_nMainContrast = m_main_contrast;
        retval.m_nShadowHue = m_shadow_hue;
        retval.m_nShadowChroma = m_shadow_chroma;
        retval.m_nShadowBrightness = m_shadow_brightness;
        retval.m_nShadowContrast = m_shadow_contrast;
        retval.m_nShadowRate = m_shadow_rate;
        return retval;
      }

      public int[] to_int_array()
      {
        int[] retval = new int[9];
        retval[0] = m_main_hue;
        retval[1] = m_main_chroma;
        retval[2] = m_main_brightness;
        retval[3] = m_main_contrast;
        retval[4] = m_shadow_hue;
        retval[5] = m_shadow_chroma;
        retval[6] = m_shadow_brightness;
        retval[7] = m_shadow_contrast;
        retval[8] = m_shadow_rate;
        return retval;
      }
    };

    private MethodInfo m_get_parts_color;
    private MethodInfo m_set_parts_color;

    private int m_current_level;
    private GameObject m_color_palette_panel;
    private ColorPaletteCtrl m_color_palette_ctrl;

    private bool m_is_visible;
    private int m_current_parts;
    private int m_current_color_tab;
    private int m_current_eye_parts_tab;

    private COLOR_VALUE m_color_value;
    private COLOR_VALUE m_control_color_value;

    private Vector2 m_scroll_position;

    private bool m_color_information_switch;
    private bool m_color_edit_switch;
    private bool[] m_color_edit_setting_switch;
    private bool[] m_color_edit_color_switch;
    private bool[] m_color_edit_control_switch;
    private bool m_random_edit_switch;
    private bool[] m_random_edit_control_switch;
    private bool m_utility_edit_switch;
    private bool[] m_utility_edit_control_switch;
    private bool m_other_setting_switch;
    private bool[] m_other_setting_control_switch;

    private void clear()
    {
      m_current_level = -1;
      m_color_palette_panel = null;
      m_color_palette_ctrl = null;

      m_is_visible = false;
      m_current_parts = -2;
      m_current_color_tab = -1;
      m_current_eye_parts_tab = -1;

      m_color_value.clear();
      m_control_color_value.clear();

      m_scroll_position = Vector2.zero;

      m_color_information_switch = false;
      m_color_edit_switch = false;

      m_color_edit_setting_switch = new bool[2];
      for(int i = 0; i < m_color_edit_setting_switch.Length; i++)
      {
        m_color_edit_setting_switch[i] = false;
      }

      m_color_edit_color_switch = new bool[2];
      for(int i = 0; i < m_color_edit_color_switch.Length; i++)
      {
        m_color_edit_color_switch[i] = false;
      }

      m_color_edit_control_switch = new bool[5];
      for(int i = 0; i < m_color_edit_control_switch.Length; i++)
      {
        m_color_edit_control_switch[i] = false;
      }

      m_random_edit_switch = false;
      m_random_edit_control_switch = new bool[9];
      for(int i = 0; i < m_random_edit_control_switch.Length; i++)
      {
        m_random_edit_control_switch[i] = false;
      }

      m_utility_edit_switch = false;
      m_utility_edit_control_switch = new bool[3];
      for(int i = 0; i < m_utility_edit_control_switch.Length; i++)
      {
        m_utility_edit_control_switch[i] = false;
      }

      m_other_setting_switch = false;
      m_other_setting_control_switch = new bool[1];
      for(int i = 0; i < m_other_setting_control_switch.Length; i++)
      {
        m_other_setting_control_switch[i] = false;
      }
    }

    public void Awake()
    {
      GameObject.DontDestroyOnLoad(this);

      Type t = typeof(ColorPaletteCtrl);
      m_get_parts_color = t.GetMethod("GetPartsColor", BindingFlags.NonPublic | BindingFlags.Instance);
      m_set_parts_color = t.GetMethod("SetPartsColor", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(MaidParts.PartsColor) }, null);

      clear();
    }

    public void OnLevelWasLoaded(int param_level)
    {
      clear();

      m_current_level = param_level;

      m_color_information_switch = true;
      m_color_edit_switch = true;
      m_color_edit_setting_switch[0] = true;
      m_color_edit_color_switch[0] = true;

      for(int i = 0; i < m_color_edit_control_switch.Length; i++)
      {
        m_color_edit_control_switch[i] = true;
      }

      for(int i = 0; i < 4; i++)
      {
        m_random_edit_control_switch[i] = true;
      }

      m_utility_edit_control_switch[0] = true;
      m_other_setting_control_switch[0] = true;
    }

    public void Update()
    {
      if(is_scene_edit_level())
      {
        return;
      }

      if(m_color_palette_panel == null)
      {
        m_color_palette_panel = GameObject.Find("UI Root/ColorPalettePanel");
      }

      if((m_color_palette_panel != null) && (m_color_palette_ctrl == null))
      {
        m_color_palette_ctrl = m_color_palette_panel.GetComponent<ColorPaletteCtrl>();
      }

      if(is_available() == false)
      {
        return;
      }

      bool is_visible;

      if(ConfigMgr.Instance.IsOpenConfigPanel())
      {
        is_visible = false;
      }
      else
      {
        is_visible = m_color_palette_panel.activeInHierarchy;
      }

      if(is_visible == false)
      {
        m_is_visible = is_visible;

        return;
      }

      if((is_visible != m_is_visible) || ((int)ColorPaletteState.m_eCurrentParts != m_current_parts) || ((int)ColorPaletteState.m_eCurrentColorTab != m_current_color_tab) || ((int)ColorPaletteState.m_eCurrentEyePartsTab != m_current_eye_parts_tab))
      {
        update_color_palette(COLOR_VALUE.FromPartsColor((MaidParts.PartsColor)m_get_parts_color.Invoke(m_color_palette_ctrl, null)));

        m_control_color_value = m_color_value;
      }
      else
      {
        COLOR_VALUE color_value = COLOR_VALUE.FromPartsColor((MaidParts.PartsColor)m_get_parts_color.Invoke(m_color_palette_ctrl, null));

        if(color_value.equals(m_color_value) == false)
        {
          m_color_value = color_value;

          m_control_color_value = color_value;
        }
      }

      m_is_visible = is_visible;
      m_current_parts = (int)ColorPaletteState.m_eCurrentParts;
      m_current_color_tab = (int)ColorPaletteState.m_eCurrentColorTab;
      m_current_eye_parts_tab = (int)ColorPaletteState.m_eCurrentEyePartsTab;


      {
        bool state;

        if(m_other_setting_control_switch[0] != false)
        {
          state = calculate_window_rect().Contains(new Vector2(Input.mousePosition.x, (Screen.height - 1) - Input.mousePosition.y)) == false;
        }
        else
        {
          state = true;
        }

        if(GameMain.Instance.MainCamera.GetControl() != state)
        {
          GameMain.Instance.MainCamera.SetControl(state);
        }

        if(UICamera.InputEnable != state)
        {
          UICamera.InputEnable = state;
        }
      }
    }

    public void OnGUI()
    {
      if(is_scene_edit_level())
      {
        return;
      }

      if(is_available() == false)
      {
        return;
      }

      if(m_is_visible == false)
      {
        return;
      }

      update_panel();
    }

    private bool is_scene_edit_level()
    {
      return (m_current_level != ConstValues.SCENE_EDIT_LEVEL);
    }

    private bool is_available()
    {
      return ((m_color_palette_panel != null) && (m_color_palette_ctrl != null) && (m_get_parts_color != null) && (m_set_parts_color != null));
    }

    private void update_panel()
    {
      Rect window_rect = calculate_window_rect();

      float content_margin_left = 10.0f;
      float content_margin_right = 10.0f;
      float content_scroll_bar_width = 20.0f;
      float content_width = window_rect.width - (content_margin_left + content_margin_right + content_scroll_bar_width);
      float content_x = content_margin_left;

      int[] int_array = m_control_color_value.to_int_array();

      {
        GUIStyle label_style = GUI.skin.GetStyle("Label");
        int default_label_font_size = label_style.fontSize;
        TextAnchor default_label_alignment = label_style.alignment;
        Color default_label_text_color = label_style.normal.textColor;

        GUIStyle button_style = GUI.skin.GetStyle("Button");
        int default_button_font_size = button_style.fontSize;
        TextAnchor default_button_alignment = button_style.alignment;
        Color default_button_text_color = button_style.normal.textColor;

        GUIStyle toggle_style = GUI.skin.GetStyle("Toggle");
        int default_toggle_font_size = toggle_style.fontSize;
        TextAnchor default_toggle_alignment = toggle_style.alignment;
        Color default_toggle_text_color = toggle_style.normal.textColor;

        label_style.fontSize = ConstValues.LABEL_FONT_SIZE;
        label_style.alignment = ConstValues.LABEL_ALIGNMENT;
        label_style.normal.textColor = ConstValues.LABEL_TEXT_COLOR;

        button_style.fontSize = ConstValues.BUTTON_FONT_SIZE;
        button_style.alignment = ConstValues.BUTTON_ALIGNMENT;
        button_style.normal.textColor = ConstValues.BUTTON_TEXT_COLOR;

        toggle_style.fontSize = ConstValues.TOGGLE_FONT_SIZE;
        toggle_style.alignment = ConstValues.TOGGLE_ALIGNMENT;
        toggle_style.normal.textColor = ConstValues.TOGGLE_TEXT_COLOR;

        float content_main_y = 0;

        GUI.BeginGroup(window_rect);

        GUI.Box(new Rect(0.0f, 0.0f, window_rect.width, window_rect.height), "");

        {
          GUI.Label(new Rect(content_x, content_main_y, 200.0f, 20.0f), ConstValues.PLUGIN_NAME + "(ver" + ConstValues.PLUGIN_VERSION + ")");

          content_main_y += 20.0f;
        }

        {
          {
            m_color_information_switch = GUI.Toggle(new Rect(content_x - 5.0f, content_main_y, content_width, 20.0f), m_color_information_switch, ConstValues.COLOR_INFORMATION_TITLE);

            content_main_y += 20.0f;
          }

          if(m_color_information_switch != false)
          {
            if(m_color_edit_switch != false)
            {
              {
                GUIStyle style = GUI.skin.GetStyle("Button");

                Texture2D default_normal_background = style.normal.background;
                Texture2D default_hover_background = style.hover.background;
                Texture2D default_active_background = style.active.background;

                style.normal.background = null;
                style.hover.background = null;
                style.active.background = null;

                if(GUI.Button(new Rect(content_x, content_main_y, 85.0f, 150.0f), ""))
                {
                  m_color_edit_color_switch[0] = true;
                  m_color_edit_color_switch[1] = false;
                }

                style.normal.background = default_normal_background;
                style.hover.background = default_hover_background;
                style.active.background = default_active_background;
              }

              {
                GUIStyle style = GUI.skin.GetStyle("Button");

                Texture2D default_normal_background = style.normal.background;
                Texture2D default_hover_background = style.hover.background;
                Texture2D default_active_background = style.active.background;

                style.normal.background = null;
                style.hover.background = null;
                style.active.background = null;

                if(GUI.Button(new Rect(content_x + 90.0f, content_main_y, 85.0f, 150.0f), ""))
                {
                  m_color_edit_color_switch[0] = false;
                  m_color_edit_color_switch[1] = true;
                }

                style.normal.background = default_normal_background;
                style.hover.background = default_hover_background;
                style.active.background = default_active_background;
              }
            }

            {
              {
                GUI.Label(new Rect(content_x, content_main_y, 90.0f, 20.0f), ConstValues.COLOR_INFORMATION_MAIN_COLOR);

                Color default_background_color = GUI.backgroundColor;

                GUIStyle style = GUI.skin.GetStyle("Box");

                Texture2D default_normal_background = style.normal.background;

                style.normal.background = Texture2D.whiteTexture;

                GUI.backgroundColor = multiple_brightness_and_chroma(hue_to_rgb(int_array[0] * (360.0f / (float)ConstValues.COLOR_PARAMETERS[0].m_max)), int_array[2], int_array[1]);

                GUI.Box(new Rect(content_x, content_main_y + 25.0f, 90.0f, 20.0f), "");

                style.normal.background = default_normal_background;

                GUI.backgroundColor = default_background_color;
              }

              {
                GUI.Label(new Rect(content_x + 95.0f, content_main_y, 90.0f, 20.0f), ConstValues.COLOR_INFORMATION_SHADOW_COLOR);

                Color default_background_color = GUI.backgroundColor;

                GUIStyle style = GUI.skin.GetStyle("Box");

                Texture2D default_normal_background = style.normal.background;

                style.normal.background = Texture2D.whiteTexture;

                GUI.backgroundColor = multiple_brightness_and_chroma(hue_to_rgb(int_array[4] * (360.0f / (float)ConstValues.COLOR_PARAMETERS[4].m_max)), int_array[6], int_array[5]);

                GUI.Box(new Rect(content_x + 95.0f, content_main_y + 25.0f, 90.0f, 20.0f), "");

                style.normal.background = default_normal_background;

                GUI.backgroundColor = default_background_color;
              }

              content_main_y += 50;
            }

            {
              for(int i = 0; i < 8; i++)
              {
                int r = i % 4;
                int c = i / 4;

                GUI.Label(new Rect(content_x + (95.0f * c), content_main_y + (20.0f * r), 90.0f, 20.0f), ConstValues.COLOR_PARAMETERS[i].m_name + ":" + int_array[i]);
              }

              {
                int i = 8;

                GUI.Label(new Rect(content_x, content_main_y + 80.0f, 90.0f, 20.0f), ConstValues.COLOR_PARAMETERS[i].m_name + ":" + int_array[i]);
              }
            }

            content_main_y += 100.0f;
          }

          content_main_y += 5.0f;
        }

        {
          float content_sub_y = 0;

          m_scroll_position = GUI.BeginScrollView(new Rect(0.0f, content_main_y, window_rect.width, window_rect.height - content_main_y), m_scroll_position, new Rect(0.0f, 0.0f, window_rect.width - content_scroll_bar_width, calculate_sub_content_height()));

          {
            {
              m_color_edit_switch = GUI.Toggle(new Rect(content_x - 5.0f, content_sub_y, content_width, 20.0f), m_color_edit_switch, ConstValues.COLOR_EDIT_TITLE);

              content_sub_y += 20.0f;
            }

            if(m_color_edit_switch != false)
            {
              {
                {
                  GUI.Label(new Rect(content_x, content_sub_y, content_width, 20.0f), ConstValues.COLOR_EDIT_SETTING_TITLE);

                  content_sub_y += 20.0f;
                }

                {
                  m_color_edit_setting_switch[0] = GUI.Toggle(new Rect(content_x, content_sub_y, content_width, 20.0f), m_color_edit_setting_switch[0], ConstValues.COLOR_EDIT_SETTING_USE_SLIDER);

                  m_color_edit_setting_switch[1] = GUI.Toggle(new Rect(content_x, content_sub_y + 20.0f, content_width, 20.0f), m_color_edit_setting_switch[1], ConstValues.COLOR_EDIT_SETTING_USE_COLOR_SET);

                  content_sub_y += 45.0f;
                }
              }

              {
                {
                  GUI.Label(new Rect(content_x, content_sub_y, content_width, 20.0f), ConstValues.COLOR_EDIT_COLOR_TITLE);

                  content_sub_y += 20.0f;
                }

                {
                  bool[] color_edit_color_switch = new bool[2];

                  color_edit_color_switch[0] = GUI.Toggle(new Rect(content_x, content_sub_y, 85.0f, 20.0f), m_color_edit_color_switch[0], ConstValues.COLOR_EDIT_COLOR_MAIN);

                  color_edit_color_switch[1] = GUI.Toggle(new Rect(content_x + 90.0f, content_sub_y, 85.0f, 20.0f), m_color_edit_color_switch[1], ConstValues.COLOR_EDIT_COLOR_SHADOW);

                  if((color_edit_color_switch[0] != m_color_edit_color_switch[0]) && (color_edit_color_switch[0] == true))
                  {
                    m_color_edit_color_switch[0] = true;
                    m_color_edit_color_switch[1] = false;
                  }
                  else if((color_edit_color_switch[1] != m_color_edit_color_switch[1]) && (color_edit_color_switch[1] == true))
                  {
                    m_color_edit_color_switch[0] = false;
                    m_color_edit_color_switch[1] = true;
                  }

                  content_sub_y += 25.0f;
                }
              }

              {
                {
                  int i_begin = 0;
                  int i_end = 4;
                  int h = 0;

                  if(m_color_edit_color_switch[1] != false)
                  {
                    i_begin = 4;
                    i_end = 8;
                    h = 4;
                  }

                  for(int i = i_begin; i < i_end; i++)
                  {
                    int r = i % 4;

                    {
                      m_color_edit_control_switch[r] = GUI.Toggle(new Rect(content_x, content_sub_y, content_width, 20.0f), m_color_edit_control_switch[r], ConstValues.COLOR_PARAMETERS[i].m_name + ":" + int_array[i]);

                      content_sub_y += 25.0f;
                    }

                    if(m_color_edit_control_switch[r] != false)
                    {
                      if(m_color_edit_setting_switch[0] != false)
                      {
                        int_array[i] = update_color_slider(ConstValues.COLOR_PARAMETERS[i], int_array[i], content_x, content_sub_y, content_width);

                        content_sub_y += 20.0f;
                      }

                      {
                        int_array[i] = update_color_adjust_set(ConstValues.COLOR_PARAMETERS[i], int_array[i], content_x, content_sub_y, content_width);

                        content_sub_y += 25.0f;
                      }

                      if(m_color_edit_setting_switch[1] != false)
                      {
                        if(r == 0)
                        {
                          int_array[i] = update_hue_color_set(ConstValues.COLOR_PARAMETERS[i], int_array[i], content_x, content_sub_y, content_width);

                          content_sub_y += 150.0f;
                        }
                        else if(r == 1)
                        {
                          int_array[i] = update_chroma_color_set(ConstValues.COLOR_PARAMETERS[i], int_array[h], int_array[i], content_x, content_sub_y, content_width);

                          content_sub_y += 100.0f;
                        }
                        else if(r == 2)
                        {
                          int_array[i] = update_brightness_color_set(ConstValues.COLOR_PARAMETERS[i], int_array[h], int_array[i], content_x, content_sub_y, content_width);

                          content_sub_y += 100.0f;
                        }
                        else if(r == 3)
                        {
                          int_array[i] = update_contrast_color_set(ConstValues.COLOR_PARAMETERS[i], int_array[i], content_x, content_sub_y, content_width);

                          content_sub_y += 50.0f;
                        }
                      }
                    }
                  }
                }

                {
                  int i = 8;
                  int r = 4;

                  {
                    m_color_edit_control_switch[r] = GUI.Toggle(new Rect(content_x, content_sub_y, content_width, 20.0f), m_color_edit_control_switch[r], ConstValues.COLOR_PARAMETERS[i].m_name + ":" + int_array[i]);

                    content_sub_y += 25.0f;
                  }

                  if(m_color_edit_control_switch[r] != false)
                  {
                    if(m_color_edit_setting_switch[0] != false)
                    {
                      int_array[i] = update_color_slider(ConstValues.COLOR_PARAMETERS[i], int_array[i], content_x, content_sub_y, content_width);

                      content_sub_y += 20.0f;
                    }

                    {
                      int_array[i] = update_color_adjust_set(ConstValues.COLOR_PARAMETERS[i], int_array[i], content_x, content_sub_y, content_width);

                      content_sub_y += 25.0f;
                    }

                    if(m_color_edit_setting_switch[1] != false)
                    {
                      int_array[i] = update_shadow_rate_color_set(ConstValues.COLOR_PARAMETERS[i], int_array[i], content_x, content_sub_y, content_width);

                      content_sub_y += 50.0f;
                    }
                  }
                }
              }
            }

            content_sub_y += 5.0f;
          }

          {
            {
              m_random_edit_switch = GUI.Toggle(new Rect(content_x - 5.0f, content_sub_y, content_width, 20.0f), m_random_edit_switch, ConstValues.RANDOM_EDIT_TITLE);

              content_sub_y += 20.0f;
            }

            if(m_random_edit_switch != false)
            {
              for(int i = 0; i < 8; i++)
              {
                int r = i % 4;
                int c = i / 4;

                m_random_edit_control_switch[i] = GUI.Toggle(new Rect(content_x + (85.0f * c), content_sub_y + (20.0f * r), 80.0f, 20.0f), m_random_edit_control_switch[i], ConstValues.COLOR_PARAMETERS[i].m_name);
              }

              {
                int i = 8;

                m_random_edit_control_switch[i] = GUI.Toggle(new Rect(content_x, content_sub_y + 80.0f, content_width, 20.0f), m_random_edit_control_switch[i], ConstValues.COLOR_PARAMETERS[i].m_name);
              }

              if(GUI.Button(new Rect(content_x, content_sub_y + 105.0f, content_width, 20.0f), ConstValues.RANDOM_EDIT_EXECUTE))
              {
                for(int i = 0; i < m_random_edit_control_switch.Length; i++)
                {
                  if(m_random_edit_control_switch[i])
                  {
                    ConstValues.COLOR_PARAMETER param = ConstValues.COLOR_PARAMETERS[i];

                    int_array[i] = UnityEngine.Random.Range(param.m_min, param.m_max);
                  }
                }
              }

              content_sub_y += 130.0f;
            }

            content_sub_y += 5.0f;
          }

          {
            {
              m_utility_edit_switch = GUI.Toggle(new Rect(content_x - 5.0f, content_sub_y, content_width, 20.0f), m_utility_edit_switch, ConstValues.UTILITY_EDIT_TITLE);

              content_sub_y += 20.0f;
            }

            if(m_utility_edit_switch != false)
            {
              {
                bool[] utility_edit_control_switch = new bool[3];

                utility_edit_control_switch[0] = GUI.Toggle(new Rect(content_x, content_sub_y, content_width, 20.0f), m_utility_edit_control_switch[0], ConstValues.UTILITY_EDIT_COPY_MAIN_TO_SHADOW);

                utility_edit_control_switch[1] = GUI.Toggle(new Rect(content_x, content_sub_y + 20.0f, content_width, 20.0f), m_utility_edit_control_switch[1], ConstValues.UTILITY_EDIT_COPY_SHADOW_TO_MAIN);

                utility_edit_control_switch[2] = GUI.Toggle(new Rect(content_x, content_sub_y + 40.0f, content_width, 20.0f), m_utility_edit_control_switch[2], ConstValues.UTILITY_EDIT_SWAP_MAIN_FOR_SHADOW);

                if((utility_edit_control_switch[0] != m_utility_edit_control_switch[0]) && (utility_edit_control_switch[0] == true))
                {
                  m_utility_edit_control_switch[0] = true;
                  m_utility_edit_control_switch[1] = false;
                  m_utility_edit_control_switch[2] = false;
                }
                else if((utility_edit_control_switch[1] != m_utility_edit_control_switch[1]) && (utility_edit_control_switch[1] == true))
                {
                  m_utility_edit_control_switch[0] = false;
                  m_utility_edit_control_switch[1] = true;
                  m_utility_edit_control_switch[2] = false;
                }
                else if((utility_edit_control_switch[2] != m_utility_edit_control_switch[2]) && (utility_edit_control_switch[2] == true))
                {
                  m_utility_edit_control_switch[0] = false;
                  m_utility_edit_control_switch[1] = false;
                  m_utility_edit_control_switch[2] = true;
                }

                content_sub_y += 65.0f;
              }

              if(GUI.Button(new Rect(content_x, content_sub_y, content_width, 20.0f), ConstValues.UTILITY_EDIT_EXECUTE))
              {
                if(m_utility_edit_control_switch[0] != false)
                {
                  int_array[4] = int_array[0];
                  int_array[5] = int_array[1];
                  int_array[6] = int_array[2];
                  int_array[7] = int_array[3];
                }

                if(m_utility_edit_control_switch[1] != false)
                {
                  int_array[0] = int_array[4];
                  int_array[1] = int_array[5];
                  int_array[2] = int_array[6];
                  int_array[3] = int_array[7];
                }

                if(m_utility_edit_control_switch[2] != false)
                {
                  int t1 = int_array[4];
                  int t2 = int_array[5];
                  int t3 = int_array[6];
                  int t4 = int_array[7];

                  int_array[4] = int_array[0];
                  int_array[5] = int_array[1];
                  int_array[6] = int_array[2];
                  int_array[7] = int_array[3];

                  int_array[0] = t1;
                  int_array[1] = t2;
                  int_array[2] = t3;
                  int_array[3] = t4;
                }
              }

              content_sub_y += 25.0f;
            }

            content_sub_y += 5.0f;
          }

          {
            {
              m_other_setting_switch = GUI.Toggle(new Rect(content_x - 5.0f, content_sub_y, content_width, 20.0f), m_other_setting_switch, ConstValues.OTHER_SETTING_TITLE);

              content_sub_y += 20.0f;
            }

            if(m_other_setting_switch != false)
            {
              m_other_setting_control_switch[0] = GUI.Toggle(new Rect(content_x, content_sub_y, content_width, 20.0f), m_other_setting_control_switch[0], ConstValues.OTHER_SETTING_DISABLE_GAME_CONTROL);

              content_sub_y += 20.0f;
            }

            content_sub_y += 5.0f;
          }

          GUI.EndScrollView();
        }

        GUI.EndGroup();

        label_style.fontSize = default_label_font_size;
        label_style.alignment = default_label_alignment;
        label_style.normal.textColor = default_label_text_color;

        button_style.fontSize = default_button_font_size;
        button_style.alignment = default_button_alignment;
        button_style.normal.textColor = default_button_text_color;

        toggle_style.fontSize = default_toggle_font_size;
        toggle_style.alignment = default_toggle_alignment;
        toggle_style.normal.textColor = default_toggle_text_color;
      }

      COLOR_VALUE color_value = COLOR_VALUE.FromIntArray(int_array);

      if(color_value.equals(m_control_color_value) == false)
      {
        update_color_palette(color_value);

        m_control_color_value = color_value;
      }
    }

    private int calculate_sub_content_height()
    {
      int retval = 0;

      if(m_color_edit_switch != false)
      {
        retval += 135;

        for(int i = 0; i < m_color_edit_control_switch.Length; i++)
        {
          if(m_color_edit_control_switch[i] != false)
          {
            retval += 50;

            if(m_color_edit_setting_switch[0] != false)
            {
              retval += 20;
            }

            if(m_color_edit_setting_switch[1] != false)
            {
              if(i == 0)
              {
                retval += 150;
              }
              else if(i == 1)
              {
                retval += 100;
              }
              else if(i == 2)
              {
                retval += 100;
              }
              else if(i == 3)
              {
                retval += 50;
              }
              else if(i == 4)
              {
                retval += 50;
              }
            }
          }
          else
          {
            retval += 25;
          }
        }
      }
      else
      {
        retval += 25;
      }

      if(m_random_edit_switch != false)
      {
        retval += 155;
      }
      else
      {
        retval += 25;
      }

      if(m_utility_edit_switch != false)
      {
        retval += 115;
      }
      else
      {
        retval += 25;
      }

      if(m_other_setting_switch != false)
      {
        retval += 45;
      }
      else
      {
        retval += 25;
      }

      return retval;
    }

    private int update_color_slider(ConstValues.COLOR_PARAMETER param_color_parameter, int param_value, float param_x, float param_y, float param_width)
    {
      return update_color_slider(param_color_parameter.m_min, param_color_parameter.m_max, param_value, param_x, param_y, param_width);
    }

    private int update_color_slider(int param_min, int param_max, int param_value, float param_x, float param_y, float param_width)
    {
      return (int)GUI.HorizontalSlider(new Rect(param_x, param_y, param_width, 20.0f), (float)param_value, (float)param_min, (float)param_max);
    }

    private int update_color_adjust_set(ConstValues.COLOR_PARAMETER param_color_parameter, int param_value, float param_x, float param_y, float param_width)
    {
      return update_color_parameter(param_color_parameter.m_min, param_color_parameter.m_max, param_value, param_x, param_y, param_width);
    }

    private int update_color_parameter(int param_min, int param_max, int param_value, float param_x, float param_y, float param_width)
    {
      if(GUI.Button(new Rect(param_x, param_y, 40.0f, 20.0f), ConstValues.COLOR_EDIT_ADJUST_SET_MINUS_1))
      {
        param_value -= 1;
      }

      if(GUI.Button(new Rect(param_x + 45.0f, param_y, 40.0f, 20.0f), ConstValues.COLOR_EDIT_ADJUST_SET_PLUS_1))
      {
        param_value += 1;
      }

      if(GUI.Button(new Rect(param_x + 90.0f, param_y, 40.0f, 20.0f), ConstValues.COLOR_EDIT_ADJUST_SET_MINUS_10))
      {
        param_value -= 10;
      }

      if(GUI.Button(new Rect(param_x + 135.0f, param_y, 40.0f, 20.0f), ConstValues.COLOR_EDIT_ADJUST_SET_PLUS_10))
      {
        param_value += 10;
      }

      return Math.Min(Math.Max(param_value, param_min), param_max);
    }

    private int update_hue_color_set(ConstValues.COLOR_PARAMETER param_color_parameter, int param_value, float param_x, float param_y, float param_width)
    {
      return update_hue_color_set(param_color_parameter.m_min, param_color_parameter.m_max, param_value, param_x, param_y, param_width);
    }

    private int update_hue_color_set(int param_min, int param_max, int param_value, float param_x, float param_y, float param_width)
    {
      for(int i = 0; i < 24; i++)
      {
        int r = i / 4;
        int c = i % 4;

        int hue = (int)Math.Min(Math.Max((15.0f * i) * ((float)param_max / 360.0f), param_min), param_max);

        Color color = multiple_brightness_and_chroma(hue_to_rgb(hue * (360.0f / (float)param_max)), 256, 224);

        GUIStyle style = GUI.skin.GetStyle("Button");

        int default_font_size = style.fontSize;
        Color default_normal_text_color = style.normal.textColor;
        Color default_hover_text_color = style.hover.textColor;
        Color default_active_text_color = style.active.textColor;

        style.fontSize = ConstValues.COLOR_EDIT_COLOR_SET_BLACK_SQUARE_FONT_SIZE;
        style.normal.textColor = color;
        style.hover.textColor = color;
        style.active.textColor = color;

        if(GUI.Button(new Rect(param_x + (45.0f * c), param_y + (25.0f * r), 40.0f, 20.0f), ConstValues.COLOR_EDIT_COLOR_SET_BLACK_SQUARE_CHARACTER))
        {
          param_value = hue;
        }

        style.fontSize = default_font_size;
        style.normal.textColor = default_normal_text_color;
        style.hover.textColor = default_hover_text_color;
        style.active.textColor = default_active_text_color;
      }

      return param_value;
    }

    private int update_chroma_color_set(ConstValues.COLOR_PARAMETER param_color_parameter, int param_hue, int param_value, float param_x, float param_y, float param_width)
    {
      return update_chroma_color_set(param_color_parameter.m_min, param_color_parameter.m_max, param_hue, param_value, param_x, param_y, param_width);
    }

    private int update_chroma_color_set(int param_min, int param_max, int param_hue, int param_value, float param_x, float param_y, float param_width)
    {
      for(int i = 0; i < 16; i++)
      {
        int r = i / 4;
        int c = i % 4;

        int chroma = (int)Math.Min(Math.Max(256.0f * ((i + 1.0f) / 16.0f), param_min), param_max);

        Color color = multiple_brightness_and_chroma(hue_to_rgb(param_hue * (360.0f / 255.0f)), 256, chroma);

        GUIStyle style = GUI.skin.GetStyle("Button");

        int default_font_size = style.fontSize;
        Color default_normal_text_color = style.normal.textColor;
        Color default_hover_text_color = style.hover.textColor;
        Color default_active_text_color = style.active.textColor;

        style.fontSize = ConstValues.COLOR_EDIT_COLOR_SET_BLACK_SQUARE_FONT_SIZE;
        style.normal.textColor = color;
        style.hover.textColor = color;
        style.active.textColor = color;

        if(GUI.Button(new Rect(param_x + (45.0f * c), param_y + (25.0f * r), 40.0f, 20.0f), ConstValues.COLOR_EDIT_COLOR_SET_BLACK_SQUARE_CHARACTER))
        {
          param_value = chroma;
        }

        style.fontSize = default_font_size;
        style.normal.textColor = default_normal_text_color;
        style.hover.textColor = default_hover_text_color;
        style.active.textColor = default_active_text_color;
      }

      return param_value;
    }

    private int update_brightness_color_set(ConstValues.COLOR_PARAMETER param_color_parameter, int param_hue, int param_value, float param_x, float param_y, float param_width)
    {
      return update_brightness_color_set(param_color_parameter.m_min, param_color_parameter.m_max, param_hue, param_value, param_x, param_y, param_width);
    }

    private int update_brightness_color_set(int param_min, int param_max, int param_hue, int param_value, float param_x, float param_y, float param_width)
    {
      for(int i = 0; i < 16; i++)
      {
        int r = i / 4;
        int c = i % 4;

        int brightness = (int)Math.Min(Math.Max(512.0f * ((i + 1.0f) / 16.0f), param_min), param_max);

        Color color = multiple_brightness_and_chroma(hue_to_rgb(param_hue * (360.0f / 255.0f)), brightness, 224);

        GUIStyle style = GUI.skin.GetStyle("Button");

        int default_font_size = style.fontSize;
        Color default_normal_text_color = style.normal.textColor;
        Color default_hover_text_color = style.hover.textColor;
        Color default_active_text_color = style.active.textColor;

        style.fontSize = ConstValues.COLOR_EDIT_COLOR_SET_BLACK_SQUARE_FONT_SIZE;
        style.normal.textColor = color;
        style.hover.textColor = color;
        style.active.textColor = color;

        if(GUI.Button(new Rect(param_x + (45.0f * c), param_y + (25.0f * r), 40.0f, 20.0f), ConstValues.COLOR_EDIT_COLOR_SET_BLACK_SQUARE_CHARACTER))
        {
          param_value = brightness;
        }

        style.fontSize = default_font_size;
        style.normal.textColor = default_normal_text_color;
        style.hover.textColor = default_hover_text_color;
        style.active.textColor = default_active_text_color;
      }

      return param_value;
    }

    private int update_contrast_color_set(ConstValues.COLOR_PARAMETER param_color_parameter, int param_value, float param_x, float param_y, float param_width)
    {
      return update_contrast_color_set(param_color_parameter.m_min, param_color_parameter.m_max, param_value, param_x, param_y, param_width);
    }

    private int update_contrast_color_set(int param_min, int param_max, int param_value, float param_x, float param_y, float param_width)
    {
      for(int i = 0; i < 8; i++)
      {
        int r = i / 4;
        int c = i % 4;

        int contrast = (int)Math.Min(Math.Max(200.0f * ((i + 1.0f) / 8.0f), param_min), param_max);

        Color color = new Color(contrast / (float)param_max, contrast / (float)param_max, contrast / (float)param_max);

        GUIStyle style = GUI.skin.GetStyle("Button");

        int default_font_size = style.fontSize;
        Color default_normal_text_color = style.normal.textColor;
        Color default_hover_text_color = style.hover.textColor;
        Color default_active_text_color = style.active.textColor;

        style.fontSize = ConstValues.COLOR_EDIT_COLOR_SET_BLACK_SQUARE_FONT_SIZE;
        style.normal.textColor = color;
        style.hover.textColor = color;
        style.active.textColor = color;

        if(GUI.Button(new Rect(param_x + (45.0f * c), param_y + (25.0f * r), 40.0f, 20.0f), ConstValues.COLOR_EDIT_COLOR_SET_BLACK_SQUARE_CHARACTER))
        {
          param_value = contrast;
        }

        style.fontSize = default_font_size;
        style.normal.textColor = default_normal_text_color;
        style.hover.textColor = default_hover_text_color;
        style.active.textColor = default_active_text_color;
      }

      return param_value;
    }

    private int update_shadow_rate_color_set(ConstValues.COLOR_PARAMETER param_color_parameter, int param_value, float param_x, float param_y, float param_width)
    {
      return update_shadow_rate_color_set(param_color_parameter.m_min, param_color_parameter.m_max, param_value, param_x, param_y, param_width);
    }

    private int update_shadow_rate_color_set(int param_min, int param_max, int param_value, float param_x, float param_y, float param_width)
    {
      for(int i = 0; i < 8; i++)
      {
        int r = i / 4;
        int c = i % 4;

        int shadow_rate = (int)Math.Min(Math.Max(256.0f * ((i + 1.0f) / 8.0f), param_min), param_max);

        Color color = new Color(1.0f - (shadow_rate / (float)param_max), 1.0f - (shadow_rate / (float)param_max), 1.0f - (shadow_rate / (float)param_max));

        GUIStyle style = GUI.skin.GetStyle("Button");

        int default_font_size = style.fontSize;
        Color default_normal_text_color = style.normal.textColor;
        Color default_hover_text_color = style.hover.textColor;
        Color default_active_text_color = style.active.textColor;

        style.fontSize = ConstValues.COLOR_EDIT_COLOR_SET_BLACK_SQUARE_FONT_SIZE;
        style.normal.textColor = color;
        style.hover.textColor = color;
        style.active.textColor = color;

        if(GUI.Button(new Rect(param_x + (45.0f * c), param_y + (25.0f * r), 40.0f, 20.0f), ConstValues.COLOR_EDIT_COLOR_SET_BLACK_SQUARE_CHARACTER))
        {
          param_value = shadow_rate;
        }

        style.fontSize = default_font_size;
        style.normal.textColor = default_normal_text_color;
        style.hover.textColor = default_hover_text_color;
        style.active.textColor = default_active_text_color;
      }

      return param_value;
    }

    private void update_color_palette(COLOR_VALUE param_color_value)
    {
      m_set_parts_color.Invoke(m_color_palette_ctrl, new object[] { param_color_value.to_parts_color() });

      m_color_palette_ctrl.LoadPaletteData(param_color_value.to_parts_color());

      m_color_value = COLOR_VALUE.FromPartsColor((MaidParts.PartsColor)m_get_parts_color.Invoke(m_color_palette_ctrl, null));
    }

    private Color hue_to_rgb(float param_hue)
    {
      Color retval;

      float hue = Math.Min(Math.Max(param_hue, 0.0f), 360.0f);

      if(hue < 60.0f)
      {
        retval = new Color(1.0f, hue / 60.0f, 0.0f);
      }
      else if(hue < 120.0f)
      {
        retval = new Color(1.0f - ((hue - 60.0f) / 60.0f), 1.0f, 0.0f);
      }
      else if(hue < 180.0f)
      {
        retval = new Color(0.0f, 1.0f, (hue - 120.0f) / 60.0f);
      }
      else if(hue < 240.0f)
      {
        retval = new Color(0.0f, 1.0f - ((hue - 180.0f) / 60.0f), 1.0f);
      }
      else if(hue < 300.0f)
      {
        retval = new Color((hue - 240.0f) / 60.0f, 0.0f, 1.0f);
      }
      else
      {
        retval = new Color(1.0f, 0.0f, 1.0f - ((hue - 300.0f) / 60.0f));
      }

      return retval;
    }

    private Color multiple_brightness_and_chroma(Color param_color, int param_brightness, int param_chroma)
    {
      Color retval;

      if(param_brightness <= 255)
      {
        float brightness = param_brightness / 255.0f;
        float chroma = param_chroma / 255.0f;
        float lightness = param_brightness / 510.0f;

        float r = param_color.r * brightness;
        float g = param_color.g * brightness;
        float b = param_color.b * brightness;

        retval = new Color(
          (r * chroma) + (lightness * (1.0f - chroma)),
          (g * chroma) + (lightness * (1.0f - chroma)),
          (b * chroma) + (lightness * (1.0f - chroma))
        );
      }
      else
      {
        float brightness = (param_brightness - 255.0f) / 255.0f;
        float chroma = param_chroma / 255.0f;
        float lightness = param_brightness / 510.0f;

        float r = param_color.r + ((1.0f - param_color.r) * brightness);
        float g = param_color.g + ((1.0f - param_color.g) * brightness);
        float b = param_color.b + ((1.0f - param_color.b) * brightness);

        retval = new Color(
          (r * chroma) + (lightness * (1.0f - chroma)),
          (g * chroma) + (lightness * (1.0f - chroma)),
          (b * chroma) + (lightness * (1.0f - chroma))
        );
      }

      return retval;
    }

    private int calculate_layout_margin()
    {
      float screen_rate = (float)Screen.height / (float)Screen.width;

      float layout_rate = 9.0f / 16.0f;

      if(screen_rate <= layout_rate)
      {
        return 0;
      }

      return (Screen.height - (int)(Screen.width * layout_rate));
    }

    private float calculate_layout_scale()
    {
      return Math.Min(Screen.height / 720.0f, Screen.width / 1280.0f);
    }

    private Rect calculate_window_rect()
    {
      float layout_margin = calculate_layout_margin();
      float layout_scale = calculate_layout_scale();

      float margin_top = (layout_margin / 2.0f) + (40.0f * layout_scale) + 10.0f;
      float margin_bottom = (layout_margin / 2.0f) + (90.0f * layout_scale) + 10.0f;
      float margin_right = 10.0f;

      float width = 240.0f;
      float height = Screen.height - (margin_top + margin_bottom);
      float x = (Screen.width - 1) - (width + margin_right);
      float y = margin_top;

      return new Rect(x, y, width, height);
    }
  }
}
