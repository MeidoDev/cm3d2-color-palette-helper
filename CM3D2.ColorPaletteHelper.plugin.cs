/*
 * 使い方
 * 
 *   0.「ReiPatcher」や「UnityInjector」とかの事前準備はwikiを見てくれ
 * 
 *   1.このテキストを「CM3D2.ColorPaletteHelper.Plugin.cs」と言う名前で保存する
 * 
 *   2.保存した「CM3D2.ColorPaletteHelper.Plugin.cs」を「C:/KISS/CM3D2_KAIZOU/UnityInjector」フォルダにいれる
 * 
 *   3.下記コマンドでコンパイルする
 * 
 *   ----------コマンドプロンプトでの操作----------
 *   cd C:/KISS/CM3D2_KAIZOU/UnityInjector
 *   C:\Windows\Microsoft.NET\Framework\v3.5\csc /t:library /lib:..\CM3D2x64_Data\Managed /r:UnityInjector.dll /r:UnityEngine.dll /r:Assembly-CSharp.dll CM3D2.ColorPaletteHelper.Plugin.cs
 *   ----------------------------------------------
 * 
 *   4.「C:/KISS/CM3D2_KAIZOU/UnityInjector」フォルダ内に「CM3D2.ColorPaletteHelper.Plugin.dll」と言うファイルが作成されたら完了だ
 * 
 * 履歴
 * 
 *   ver0.1.0 初版
 */

using System;
using System.Reflection;
using UnityEngine;
using UnityInjector;
using UnityInjector.Attributes;

namespace CM3D2.ColorPaletteHelper.Plugin
{
    [PluginFilter("CM3D2x64"), PluginFilter("CM3D2x86"), PluginFilter("CM3D2VRx64"), PluginName("CM3D2 Color Palette Helper"), PluginVersion("0.1.0")]

    public class ColorPaletteHelper : PluginBase
    {
        private struct ColorPaletteValue
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

            public bool equals(ColorPaletteValue value)
            {
                return (
                    (m_main_hue == value.m_main_hue) &&
                    (m_main_chroma == value.m_main_chroma) &&
                    (m_main_brightness == value.m_main_brightness) &&
                    (m_main_contrast == value.m_main_contrast) &&
                    (m_shadow_hue == value.m_shadow_hue) &&
                    (m_shadow_chroma == value.m_shadow_chroma) &&
                    (m_shadow_brightness == value.m_shadow_brightness) &&
                    (m_shadow_contrast == value.m_shadow_contrast) &&
                    (m_shadow_rate == value.m_shadow_rate)
                );
            }
        };

        private const string version = "0.1.0";

        private int m_current_level;
        private GameObject m_color_palette_panel;
        private ColorPaletteCtrl m_color_palette_ctrl;
        private bool m_is_visible;

        private ColorPaletteValue m_color;
        private Vector2 m_scroll_position;

        private MethodInfo m_get_parts_color;
        private MethodInfo m_set_parts_color;

        public void Awake()
        {
            m_current_level = 0;
            m_color_palette_panel = null;
            m_color_palette_ctrl = null;
            m_is_visible = false;
            m_color.clear();
            m_scroll_position = Vector2.zero;

            Type t = typeof(ColorPaletteCtrl);
            m_get_parts_color = t.GetMethod("GetPartsColor", BindingFlags.NonPublic | BindingFlags.Instance);
            m_set_parts_color = t.GetMethod("SetPartsColor", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(MaidParts.PartsColor) }, null);

            GameObject.DontDestroyOnLoad(this);
        }

        public void OnLevelWasLoaded(int param_level)
        {
            m_current_level = param_level;

            m_color_palette_panel = null;
            m_color_palette_ctrl = null;
            m_is_visible = false;
        }

        public void Update()
        {
            if (m_current_level != 5)
            {
                return;
            }

            if (m_color_palette_panel == null)
            {
                m_color_palette_panel = GameObject.Find("UI Root/ColorPalettePanel");

                if (m_color_palette_panel != null)
                {
                    m_color_palette_ctrl = m_color_palette_panel.GetComponent<ColorPaletteCtrl>();
                }
            }

            if (m_color_palette_panel != null)
            {
                m_is_visible = m_color_palette_panel.activeInHierarchy;
            }

            if ((m_is_visible) && (m_get_parts_color != null))
            {
                MaidParts.PartsColor c = (MaidParts.PartsColor)m_get_parts_color.Invoke(m_color_palette_ctrl, null);

                m_color.m_main_hue = c.m_nMainHue;
                m_color.m_main_chroma = c.m_nMainChroma;
                m_color.m_main_brightness = c.m_nMainBrightness;
                m_color.m_main_contrast = c.m_nMainContrast;
                m_color.m_shadow_hue = c.m_nShadowHue;
                m_color.m_shadow_chroma = c.m_nShadowChroma;
                m_color.m_shadow_brightness = c.m_nShadowBrightness;
                m_color.m_shadow_contrast = c.m_nShadowContrast;
                m_color.m_shadow_rate = c.m_nShadowRate;
            }
        }

        public void OnGUI()
        {
            if (m_current_level != 5)
            {
                return;
            }

            if (m_is_visible == false)
            {
                return;
            }

            update_panel();
        }

        private void update_panel()
        {
            float layout_margin = get_layout_margin();
            float layout_rate = get_layout_rate();

            float window_margin_top = layout_margin + (45.0f * layout_rate) + 10.0f;
            float window_margin_bottom = layout_margin + (95.0f * layout_rate) + 10.0f;
            float window_margin_right = 10.0f;

            float window_width = 220.0f;
            float window_height = Screen.height - (window_margin_top + window_margin_bottom);
            float window_x = (Screen.width - 1.0f) - (window_width + window_margin_right);
            float window_y = window_margin_top;

            float control_horizontal_margin = 10.0f;
            float control_x = control_horizontal_margin;
            float control_width = window_width - (control_horizontal_margin * 2.0f) - 20.0f;

            ColorPaletteValue color = m_color;

            Rect window_rect = new Rect(window_x, window_y, window_width, window_height);

            GUI.BeginGroup(window_rect);

            {
                Rect background_rect = new Rect(0.0f, 0.0f, window_width, window_height);

                GUIStyle background_style = "box";

                background_style.alignment = TextAnchor.UpperLeft;

                GUI.Box(background_rect, "ColorPaletteHelper(ver" + version + ")", background_style);
            }

            {
                Rect scroll_rect = new Rect(0.0f, 20.0f, window_width, window_height - 20.0f);

                Rect content_rect = new Rect(0.0f, 0.0f, window_width - 20.0f, 580.0f);

                m_scroll_position = GUI.BeginScrollView(scroll_rect, m_scroll_position, content_rect);

                {
                    float base_y = 0.0f;

                    Rect label_rect = new Rect(control_x, base_y, control_width, 20.0f);

                    GUI.Label(label_rect, "色相:" + color.m_main_hue);

                    Rect slider_rect = new Rect(control_x, base_y + 20.0f, control_width, 20.0f);

                    color.m_main_hue = (int)GUI.HorizontalSlider(slider_rect, (float)color.m_main_hue, 0.0f, 255.0f);

                    Rect button_m1_rect = new Rect(control_x, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_m1_rect, "-1"))
                    {
                        color.m_main_hue -= 1;
                    }

                    Rect button_p1_rect = new Rect(control_x + 45.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_p1_rect, "+1"))
                    {
                        color.m_main_hue += 1;
                    }

                    Rect button_m10_rect = new Rect(control_x + 90.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_m10_rect, "-10"))
                    {
                        color.m_main_hue -= 10;
                    }

                    Rect button_p10_rect = new Rect(control_x + 135.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_p10_rect, "+10"))
                    {
                        color.m_main_hue += 10;
                    }

                    if (color.m_main_hue < 0)
                    {
                        color.m_main_hue = 0;
                    }

                    if (color.m_main_hue > 255)
                    {
                        color.m_main_hue = 255;
                    }
                }

                {
                    float base_y = 60.0f;

                    Rect label_rect = new Rect(control_x, base_y, control_width, 20.0f);

                    GUI.Label(label_rect, "彩度:" + color.m_main_chroma);

                    Rect slider_rect = new Rect(control_x, base_y + 20.0f, control_width, 20.0f);

                    color.m_main_chroma = (int)GUI.HorizontalSlider(slider_rect, (float)color.m_main_chroma, 0.0f, 255.0f);

                    Rect button_m1_rect = new Rect(control_x, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_m1_rect, "-1"))
                    {
                        color.m_main_chroma -= 1;
                    }

                    Rect button_p1_rect = new Rect(control_x + 45.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_p1_rect, "+1"))
                    {
                        color.m_main_chroma += 1;
                    }

                    Rect button_m10_rect = new Rect(control_x + 90.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_m10_rect, "-10"))
                    {
                        color.m_main_chroma -= 10;
                    }

                    Rect button_p10_rect = new Rect(control_x + 135.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_p10_rect, "+10"))
                    {
                        color.m_main_chroma += 10;
                    }

                    if (color.m_main_chroma < 0)
                    {
                        color.m_main_chroma = 0;
                    }

                    if (color.m_main_chroma > 255)
                    {
                        color.m_main_chroma = 255;
                    }
                }

                {
                    float base_y = 120.0f;

                    Rect label_rect = new Rect(control_x, base_y, control_width, 20.0f);

                    GUI.Label(label_rect, "明度:" + color.m_main_brightness);

                    Rect slider_rect = new Rect(control_x, base_y + 20.0f, control_width, 20.0f);

                    color.m_main_brightness = (int)GUI.HorizontalSlider(slider_rect, (float)color.m_main_brightness, 0.0f, 510.0f);

                    Rect button_m1_rect = new Rect(control_x, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_m1_rect, "-1"))
                    {
                        color.m_main_brightness -= 1;
                    }

                    Rect button_p1_rect = new Rect(control_x + 45.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_p1_rect, "+1"))
                    {
                        color.m_main_brightness += 1;
                    }

                    Rect button_m10_rect = new Rect(control_x + 90.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_m10_rect, "-10"))
                    {
                        color.m_main_brightness -= 10;
                    }

                    Rect button_p10_rect = new Rect(control_x + 135.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_p10_rect, "+10"))
                    {
                        color.m_main_brightness += 10;
                    }

                    if (color.m_main_brightness < 0)
                    {
                        color.m_main_brightness = 0;
                    }

                    if (color.m_main_brightness > 510)
                    {
                        color.m_main_brightness = 510;
                    }
                }

                {
                    float base_y = 180.0f;

                    Rect label_rect = new Rect(control_x, base_y, control_width, 20.0f);

                    GUI.Label(label_rect, "対照:" + color.m_main_contrast);

                    Rect slider_rect = new Rect(control_x, base_y + 20.0f, control_width, 20.0f);

                    color.m_main_contrast = (int)GUI.HorizontalSlider(slider_rect, (float)color.m_main_contrast, 0.0f, 200.0f);

                    Rect button_m1_rect = new Rect(control_x, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_m1_rect, "-1"))
                    {
                        color.m_main_contrast -= 1;
                    }

                    Rect button_p1_rect = new Rect(control_x + 45.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_p1_rect, "+1"))
                    {
                        color.m_main_contrast += 1;
                    }

                    Rect button_m10_rect = new Rect(control_x + 90.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_m10_rect, "-10"))
                    {
                        color.m_main_contrast -= 10;
                    }

                    Rect button_p10_rect = new Rect(control_x + 135.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_p10_rect, "+10"))
                    {
                        color.m_main_contrast += 10;
                    }

                    if (color.m_main_contrast < 0)
                    {
                        color.m_main_contrast = 0;
                    }

                    if (color.m_main_contrast > 200)
                    {
                        color.m_main_contrast = 200;
                    }
                }

                {
                    float base_y = 240.0f;

                    Rect label_rect = new Rect(control_x, base_y, control_width, 20.0f);

                    GUI.Label(label_rect, "色相(影):" + color.m_shadow_hue);

                    Rect slider_rect = new Rect(control_x, base_y + 20.0f, control_width, 20.0f);

                    color.m_shadow_hue = (int)GUI.HorizontalSlider(slider_rect, (float)color.m_shadow_hue, 0.0f, 255.0f);

                    Rect button_m1_rect = new Rect(control_x, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_m1_rect, "-1"))
                    {
                        color.m_shadow_hue -= 1;
                    }

                    Rect button_p1_rect = new Rect(control_x + 45.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_p1_rect, "+1"))
                    {
                        color.m_shadow_hue += 1;
                    }

                    Rect button_m10_rect = new Rect(control_x + 90.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_m10_rect, "-10"))
                    {
                        color.m_shadow_hue -= 10;
                    }

                    Rect button_p10_rect = new Rect(control_x + 135.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_p10_rect, "+10"))
                    {
                        color.m_shadow_hue += 10;
                    }

                    if (color.m_shadow_hue < 0)
                    {
                        color.m_shadow_hue = 0;
                    }

                    if (color.m_shadow_hue > 255)
                    {
                        color.m_shadow_hue = 255;
                    }
                }

                {
                    float base_y = 300.0f;

                    Rect label_rect = new Rect(control_x, base_y, control_width, 20.0f);

                    GUI.Label(label_rect, "彩度(影):" + color.m_shadow_chroma);

                    Rect slider_rect = new Rect(control_x, base_y + 20.0f, control_width, 20.0f);

                    color.m_shadow_chroma = (int)GUI.HorizontalSlider(slider_rect, (float)color.m_shadow_chroma, 0.0f, 255.0f);

                    Rect button_m1_rect = new Rect(control_x, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_m1_rect, "-1"))
                    {
                        color.m_shadow_chroma -= 1;
                    }

                    Rect button_p1_rect = new Rect(control_x + 45.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_p1_rect, "+1"))
                    {
                        color.m_shadow_chroma += 1;
                    }

                    Rect button_m10_rect = new Rect(control_x + 90.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_m10_rect, "-10"))
                    {
                        color.m_shadow_chroma -= 10;
                    }

                    Rect button_p10_rect = new Rect(control_x + 135.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_p10_rect, "+10"))
                    {
                        color.m_shadow_chroma += 10;
                    }

                    if (color.m_shadow_chroma < 0)
                    {
                        color.m_shadow_chroma = 0;
                    }

                    if (color.m_shadow_chroma > 255)
                    {
                        color.m_shadow_chroma = 255;
                    }
                }

                {
                    float base_y = 360.0f;

                    Rect label_rect = new Rect(control_x, base_y, control_width, 20.0f);

                    GUI.Label(label_rect, "明度(影):" + color.m_shadow_brightness);

                    Rect slider_rect = new Rect(control_x, base_y + 20.0f, control_width, 20.0f);

                    color.m_shadow_brightness = (int)GUI.HorizontalSlider(slider_rect, (float)color.m_shadow_brightness, 0.0f, 510.0f);

                    Rect button_m1_rect = new Rect(control_x, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_m1_rect, "-1"))
                    {
                        color.m_shadow_brightness -= 1;
                    }

                    Rect button_p1_rect = new Rect(control_x + 45.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_p1_rect, "+1"))
                    {
                        color.m_shadow_brightness += 1;
                    }

                    Rect button_m10_rect = new Rect(control_x + 90.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_m10_rect, "-10"))
                    {
                        color.m_shadow_brightness -= 10;
                    }

                    Rect button_p10_rect = new Rect(control_x + 135.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_p10_rect, "+10"))
                    {
                        color.m_shadow_brightness += 10;
                    }

                    if (color.m_shadow_brightness < 0)
                    {
                        color.m_shadow_brightness = 0;
                    }

                    if (color.m_shadow_brightness > 510)
                    {
                        color.m_shadow_brightness = 510;
                    }
                }

                {
                    float base_y = 420.0f;

                    Rect label_rect = new Rect(control_x, base_y, control_width, 20.0f);

                    GUI.Label(label_rect, "対照(影):" + color.m_shadow_contrast);

                    Rect slider_rect = new Rect(control_x, base_y + 20.0f, control_width, 20.0f);

                    color.m_shadow_contrast = (int)GUI.HorizontalSlider(slider_rect, (float)color.m_shadow_contrast, 0.0f, 200.0f);

                    Rect button_m1_rect = new Rect(control_x, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_m1_rect, "-1"))
                    {
                        color.m_shadow_contrast -= 1;
                    }

                    Rect button_p1_rect = new Rect(control_x + 45.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_p1_rect, "+1"))
                    {
                        color.m_shadow_contrast += 1;
                    }

                    Rect button_m10_rect = new Rect(control_x + 90.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_m10_rect, "-10"))
                    {
                        color.m_shadow_contrast -= 10;
                    }

                    Rect button_p10_rect = new Rect(control_x + 135.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_p10_rect, "+10"))
                    {
                        color.m_shadow_contrast += 10;
                    }

                    if (color.m_shadow_contrast < 0)
                    {
                        color.m_shadow_contrast = 0;
                    }

                    if (color.m_shadow_contrast > 200)
                    {
                        color.m_shadow_contrast = 200;
                    }
                }

                {
                    float base_y = 480.0f;

                    Rect label_rect = new Rect(control_x, base_y, control_width, 20.0f);

                    GUI.Label(label_rect, "影率:" + color.m_shadow_rate);

                    Rect slider_rect = new Rect(control_x, base_y + 20.0f, control_width, 20.0f);

                    color.m_shadow_rate = (int)GUI.HorizontalSlider(slider_rect, (float)color.m_shadow_rate, 0.0f, 255.0f);

                    Rect button_m1_rect = new Rect(control_x, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_m1_rect, "-1"))
                    {
                        color.m_shadow_rate -= 1;
                    }

                    Rect button_p1_rect = new Rect(control_x + 45.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_p1_rect, "+1"))
                    {
                        color.m_shadow_rate += 1;
                    }

                    Rect button_m10_rect = new Rect(control_x + 90.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_m10_rect, "-10"))
                    {
                        color.m_shadow_rate -= 10;
                    }

                    Rect button_p10_rect = new Rect(control_x + 135.0f, base_y + 40.0f, 40.0f, 20.0f);

                    if (GUI.Button(button_p10_rect, "+10"))
                    {
                        color.m_shadow_rate += 10;
                    }

                    if (color.m_shadow_rate < 0)
                    {
                        color.m_shadow_rate = 0;
                    }

                    if (color.m_shadow_rate > 255)
                    {
                        color.m_shadow_rate = 255;
                    }
                }

                GUI.EndScrollView();
            }

            GUI.EndGroup();

            if ((m_color_palette_ctrl != null) && (!color.equals(m_color)))
            {
                MaidParts.PartsColor c;

                c.m_bUse = true;
                c.m_nMainHue = color.m_main_hue;
                c.m_nMainChroma = color.m_main_chroma;
                c.m_nMainBrightness = color.m_main_brightness;
                c.m_nMainContrast = color.m_main_contrast;
                c.m_nShadowHue = color.m_shadow_hue;
                c.m_nShadowChroma = color.m_shadow_chroma;
                c.m_nShadowBrightness = color.m_shadow_brightness;
                c.m_nShadowContrast = color.m_shadow_contrast;
                c.m_nShadowRate = color.m_shadow_rate;

                if (m_set_parts_color != null)
                {
                    m_set_parts_color.Invoke(m_color_palette_ctrl, new object[] { c });
                }

                m_color_palette_ctrl.LoadPaletteData(c);

                if (m_set_parts_color != null)
                {
                    m_set_parts_color.Invoke(m_color_palette_ctrl, new object[] { c });
                }

                m_color_palette_ctrl.LoadPaletteData(c);

                m_color = color;
            }
        }

        private int get_layout_margin()
        {
            float screen_rate = (float)Screen.height / (float)Screen.width;

            float layout_rate = 9.0f / 16.0f;

            if (screen_rate <= layout_rate)
            {
                return 0;
            }

            return (Screen.height - (int)((float)Screen.width * layout_rate)) / 2;
        }

        private float get_layout_rate()
        {
            return (float)Screen.width / 1280.0f;
        }
    }
}