using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public static class VLib
{
    static string[] m_consonants = { "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "r", "s", "t", "v", "w", "z", };// "x", "q", "ch", "kh", "gh" };
    static string[] m_vowels = { "a", "e", "i", "o", "u" };//, "oo", "ou", "ee" };

    internal static Color COLOR_yellow = new Color(1f,188f/255f,0f);
    public static string GetEnumName<T>(T a_type) { return Enum.GetName(typeof(T), a_type); }

    public const float _degreesToRadians = 3.14159265358979323f / 180f;

    public const float _msToKmh = 3.6f;

    public static string FirstLetterToUpperCaseOrConvertNullToEmptyString(this string s)
    {
        if (string.IsNullOrEmpty(s))
            return string.Empty;

        char[] a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }

    public static float TruncateFloatsDecimalPlaces(float a_float, int a_decimalsToKeep)
    {
        float multiplier = Mathf.Pow(10, a_decimalsToKeep);
        float truncatedFloat = ((int)(a_float * multiplier) / multiplier);

        return truncatedFloat;
    }

    #region Clamps

    public static float BigClamp(float a_value, float a_ceiling)
    {
        return a_value > a_ceiling ? a_ceiling : a_value;
    }

    public static int BigClamp(int a_value, int a_ceiling)
    {
        return a_value > a_ceiling ? a_ceiling : a_value;
    }

    public static double BigClamp(double a_value, double a_ceiling)
    {
        return a_value > a_ceiling ? a_ceiling : a_value;
    }

    public static float LilClamp(float a_value, float a_floor)
    {
        return a_value < a_floor ? a_floor : a_value;
    }

    public static int LilClamp(int a_value, int a_floor)
    {
        return a_value < a_floor ? a_floor : a_value;
    }

    public static double LilClamp(double a_value, double a_floor)
    {
        return a_value < a_floor ? a_floor : a_value;
    }

    #endregion

    public static float SigmoidLerp(float a_start, float a_finish, float a_t, int a_sensitivity = 3)
    {
        float retVal = 0f;
        float x = a_t;
        int y = a_sensitivity;
        if (a_t <= 0.5)
        {
            retVal = Mathf.Pow(x, (float)y) *(float)(2 << ((int)y - 2));
        }
        else
        {
            retVal = (float)(Mathf.Pow(-2, y - 1)) * Mathf.Pow(x - 1f, (float)y) + 1f;
        }

        retVal = a_start + (a_finish - a_start) * retVal;

        return retVal;
    }

    public static Vector2 SigmoidLerp(Vector2 a_start, Vector3 a_finish, float a_t, int a_sensitivity = 3)
    {
        Vector2 retVec = new Vector2();
        retVec.x = SigmoidLerp(a_start.x, a_finish.x, a_t, a_sensitivity);
        retVec.y = SigmoidLerp(a_start.y, a_finish.y, a_t, a_sensitivity);
        return retVec;
    }

    public static Vector3 SigmoidLerp(Vector3 a_start, Vector3 a_finish, float a_t, int a_sensitivity = 3)
    {
        Vector3 retVec = new Vector3();
        retVec.x = SigmoidLerp(a_start.x, a_finish.x, a_t, a_sensitivity);
        retVec.y = SigmoidLerp(a_start.y, a_finish.y, a_t, a_sensitivity);
        retVec.z = SigmoidLerp(a_start.z, a_finish.z, a_t, a_sensitivity);
        return retVec;
    }

    public static float Eerp(float a_start, float a_end, float a_time, float a_exponent)
    {
        float retVal = Mathf.Lerp(a_start,a_end,Mathf.Pow(a_time,a_exponent));
        return retVal;
    }

    public static float ClampRotation(float a_rotation)
    {
        float result = a_rotation;
        bool modified = true;
        while (modified)
        {
            modified = false;
            if (result > 180f)
            {
                result -= 360f;
                modified = true;
            }
            else if (result < -180f)
            {
                result += 360f;
                modified = true;
            }
        }
        return result;
    }

    public static Vector2 EulerAngleToVector2(float a_angle)
    {
        float x = Mathf.Sin(a_angle * Mathf.PI / 180f);
        float y = Mathf.Cos(a_angle * Mathf.PI / 180f);
        return new Vector2(x, y);
    }

    public static Vector3 Euler2dAngleToVector3(float a_angle)
    {
        float x = -Mathf.Sin(a_angle * Mathf.PI / 180f);
        float y = Mathf.Cos(a_angle * Mathf.PI / 180f);
        float z = 0f;
        return new Vector3(x, y, z);
    }

    public static Quaternion Vector2DirectionToQuaternion(Vector2 a_vector2)
    {
        return Quaternion.Euler(0f,0f,Vector2ToEulerAngle(a_vector2));
    }

    public static Vector2 RotateVector2(this Vector2 a_vector2, float a_angle)
    {
        return RotateVector3In2D(a_vector2, a_angle);
    }

    public static Vector3 RotateVector3In2D(this Vector3 a_vector3, float a_angle)
    {
        Vector3 returnVector = a_vector3;
        returnVector = Quaternion.AngleAxis(a_angle, Vector3.forward) * returnVector;
        return returnVector;
    }
    public static Vector2 ToVector2(this Vector3 a_vector3)
    {
        Vector2 returnVector = new Vector2(a_vector3.x, a_vector3.y);
        return returnVector;
    }

    public static Vector3 ToVector3(this Vector2 a_vector3)
    {
        Vector3 returnVector = new Vector3(a_vector3.x, a_vector3.y,0f);
        return returnVector;
    }

    public static float AngleBetweenVector3s(Vector3 a_vectorA, Vector3 a_vectorB)
    {
        float angle = 0f;
        
        return angle;
    }

    public static float Vector2ToEulerAngle(Vector2 a_vector2)
    {
        float angle = Vector2.SignedAngle(new Vector2(0f,1f), a_vector2);
        return angle;
    }

    public static Vector3 Vector2ToEulerAngles(Vector2 a_vector2)
    {
        return new Vector3(0f, 0f, Vector2ToEulerAngle(a_vector2));
    }

    public static float Vector3ToEulerAngle(Vector3 a_vector3)
    {
        return Vector2ToEulerAngle(a_vector3.ToVector2());
    }

    public static Vector3 Vector3ToEulerAngles(Vector3 a_vector3)
    {
        return Vector2ToEulerAngles(a_vector3.ToVector2());
    }

    public static Quaternion Vector3ToQuaternion(Vector3 a_vector3)
    {
        return Quaternion.Euler(Vector3ToEulerAngles(a_vector3));
    }

    public static Vector3 RandomVector3Direction()
    {
        return new Vector3(vRandom(-1f,1f), vRandom(-1f, 1f), vRandom(-1f, 1f));
    }

    public static float CosDeg(float a_angle)
    {
        return Mathf.Cos(a_angle * Mathf.PI/180f);
    }

    public static int SafeMod(int a_value, int a_mod)
    {
        return (a_value % a_mod + a_mod) % a_mod;
    }

    public static int vRandom(int a_inclusiveMin, int a_inclusiveMax)
    {
        return UnityEngine.Random.Range(a_inclusiveMin, a_inclusiveMax+1);
    }

    public static float vRandom(float a_inclusiveMin, float a_inclusiveMax)
    {
        return UnityEngine.Random.Range(a_inclusiveMin, a_inclusiveMax);
    }

    public static float[] vRandomSpread(int a_count,float a_deviation)
    {
        float[] rolls = new float[a_count];

        float totalRoll = 0f;
        for (int i = 0; i < rolls.Length; i++)
        {
            rolls[i] += vRandom(0f, 100f);
            totalRoll += rolls[i];
        }

        for (int i = 0; i < rolls.Length; i++)
        {
            rolls[i] = rolls[i] / (totalRoll/2f);
            rolls[i] = (1f - a_deviation) + a_deviation * rolls[i];
        }

        return rolls;
    }

    public static Color Randomise(this Color a_color)
    {
        Color newColor = new Color();
        newColor.r = VLib.vRandom(0f, 1f);
        newColor.g = VLib.vRandom(0f, 1f);
        newColor.b = VLib.vRandom(0f, 1f);
        newColor.a = 1f;

        return newColor;
    }

    //Returns the color with specified alpha
    public static Color WithAlpha(this Color a_color, float a_alpha)
    {
        Color newColor = a_color;
        newColor.a = a_alpha;

        return newColor;
    }

    public static Color RatioToColorRGB(float a_ratio)
    {
        Color returnColor = Color.white;

        float redRatio = Mathf.Clamp(2f - (2f * a_ratio), 0f, 1f);
        float greenRatio = Mathf.Clamp(2f*a_ratio, 0f, 1f);
        returnColor = new Color(redRatio, greenRatio, 0f);
        return returnColor;
    }

    internal static Color RatioToColorRarity(float a_ratio)
    {
        Color returnColor = Color.white;
        float cases = 6f;
        float mod = (1f / cases);
        float modRatio = a_ratio % mod;
        modRatio /= mod;
        switch (a_ratio)
        {
            case float n when (n >= 0f / cases && n < 1f/ cases):
                returnColor = RatioToColorRGB(modRatio);// (1f-a_ratio) / (1f / cases));
                break;
            case float n when (n > 1f / cases && n < 2f / cases):
                returnColor = Color.Lerp(Color.green, Color.cyan, modRatio);
                break;
            case float n when (n > 2f / cases && n < 3f / cases):
                returnColor = Color.Lerp(Color.cyan, Color.blue, modRatio);
                break;
            case float n when (n > 3f / cases && n < 4f / cases):
                returnColor = Color.Lerp(Color.blue, Color.magenta, modRatio);
                break;
            case float n when (n > 4f / cases && n < 5f / cases):
                returnColor = Color.Lerp(Color.magenta, Color.white, modRatio);
                break;
            case float n when (n > 5f / cases && n < 6f / cases):
                returnColor = Color.Lerp(Color.white, Color.black, modRatio);
                break;
            default:
                break;
        }

        return returnColor;
    }

    public static Color RatioToColorRGBWithAlpha(float a_percentage)
    {
        Color returnColor = RatioToColorRGB(a_percentage);

        returnColor.a = a_percentage;// = new Color(redRatio, greenRatio, 0f, a_percentage);
        return returnColor;
    }

    public static string GenerateRandomizedName(int a_minimumLength = 2, int a_maximumLength = 10)
    {
        string name = "";
        int length = vRandom(a_minimumLength, a_maximumLength);
        int startWithVowel = vRandom(0, 1);

        for (int i = 0; i < length; i++)
        {
            if (i % 2 == startWithVowel)
            {
                name += m_consonants[vRandom(0, m_consonants.Length-1)];
            }
            else
            {
                name += m_vowels[vRandom(0, m_vowels.Length-1)];
            }
            if (i == 0)
            {
                string tempString = name;
                tempString = name[0].ToString().ToUpper();
                for (int j = 1; j < name.Length; j++)
                {
                    tempString += name[i];
                }
                name = tempString;
            }
        }
        return name;
    }

    //Returns a string[2]
    public static string[] GenerateRandomPersonsName()
    {
        string[] generatedNames = new string[2];

        string[] firstNames =
        {
            "Andrew",
            "Bart",
            "Ben",
            "John",
            "Stuart",
            "Giles",
            "Carnaby",
            "Sean",
            "Will",
            "Micky",
            "Jack",
            "Tyler",
            "Israq",
            "Daniel",
            "Stephen",
            "Richard",
            "Oliver",
            "Archie",

            "Niamh",
            "Eloise",
            "Mikenna",
            "Molly",
            "Emma",
            "Lavinia",
            "Sandra",
            "Mary",
            "Julia",
            "Tina",
            "Penelope",
            "Lucy",
            "Becky",
            "Ruth",
            "Calli",
            "Aditi",
            "Molly",
            "Emma",
            "Naomi",
            "Zara",
        };

        string[] lastNames =
        {
            "Digby",
            "Webster-Hawes",
            "Constant",
            "Vann",
            "Ison",
            "Hansen",
            "Gilani",
            "Dhrubo",
            "Modra",
            "Head",
            "Hensgen",
            "Basnet",
            "Shoebridge",
            "Porter",
            "Foster",
            "Erieau",
            "Gordon-Anderson",
            "Taylor",
            "Waters"
        };

        generatedNames[0] = firstNames[UnityEngine.Random.Range(0, firstNames.Length)];
        generatedNames[1] = lastNames[UnityEngine.Random.Range(0, lastNames.Length)];

        return generatedNames;
    }

    public static string[] GetEnumNames(Type enumType)
    {
        if (!enumType.IsEnum)
        {
            throw new ArgumentException("The provided type is not an enum.");
        }

        return Enum.GetNames(enumType);
    }

    public static float RoundToDecimalPlaces(float a_value, int a_decimalPlaces)
    {
        float result = a_value;
        float shift = Mathf.Pow(10f, a_decimalPlaces);
        result *= shift;
        result = Mathf.Round(result);
        result /= shift;
        return result;
    }

    public static string ScrambleAlphabeticalString(string a_input)
    {
        string output = string.Empty;
        int charBase = -1;
        int cypher = 1; //VLib.vRandom(0, 26);
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < a_input.Length; i++)
        {
            char c = a_input[i];
            if (c != ' ')
            {
                float roll = VLib.vRandom(0f, 1f);
                if (roll >= 0.5)
                {
                    c = '?';
                }
                else if (roll >= 0.25)
                {
                    int symbolRoll = vRandom(1, 4);
                    switch (symbolRoll)
                    {
                        case 1:
                            c = '!';
                            break;
                        case 2:
                            c = '#';
                            break;
                        case 3:
                            c = '&';
                            break;
                        case 4:
                            c = '@';
                            break;
                    }
                }
                else
                {
                    if (c > 91)
                    {
                        charBase = 97;
                    }
                    else
                    {
                        charBase = 65;
                    }
                    int cInt = c + cypher;
                    cInt = (cInt - charBase) % 26;
                    c = (char)(cInt + charBase);
                }

            }
            builder.Append(c);
        }
        output = builder.ToString();
        return output;
    }

    public static char ScrambleAlphabeticalCharacter(char a_char)
    {
        if (a_char != ' ')
        {
            int charBase = -1;
            int cypher = 1;
            float roll = VLib.vRandom(0f, 1f);
            if (roll >= 0.5)
            {
                a_char = '?';
            }
            else if (roll >= 0.25)
            {
                int symbolRoll = vRandom(1, 4);
                switch (symbolRoll)
                {
                    case 1:
                        a_char = '!';
                        break;
                    case 2:
                        a_char = '#';
                        break;
                    case 3:
                        a_char = '&';
                        break;
                    case 4:
                        a_char = '@';
                        break;
                }
            }
            else
            {
                if (a_char > 91)
                {
                    charBase = 97;
                }
                else
                {
                    charBase = 65;
                }
                int cInt = a_char + cypher;
                cInt = (cInt - charBase) % 26;
                a_char = (char)(cInt + charBase);
            }
        }
        return a_char;
    }

    public static string ScrambleRandomAlphabeticalCharacter(string a_input)
    {
        string output = string.Empty;
        if(a_input.Length > 0)
        {
            StringBuilder builder = new StringBuilder(a_input);
            int index = VLib.vRandom(0, a_input.Length - 1);
            builder[index] = ScrambleAlphabeticalCharacter(a_input[index]);
            output = builder.ToString();
        }
        return output;
    }

    internal static int CharToInt(char a_char)
    {
        int result = a_char - '0';
        return result;
    }

    internal static int StringToInt(string a_string)
    {
        int result = 0;
        int powerOfTen = 0;
        for (int i = a_string.Length-1; i >= 0; i--)
        {
            result += (int)(CharToInt(a_string[i]) * Math.Pow(10, powerOfTen));
            powerOfTen++;
        }
        return result;
    }

    internal static int[] GetApplicationVersionNumbers()
    {
        int[] versionNumbers = new int[2];
        //versionNumbers[0] = int.Parse(Application.version);
        string versionString = Application.version;
        int tenPower = 0;
        for (int i = versionString.Length-1; i >= 0; i--)
        {
            if (versionString[i] == '.')
            {
                versionNumbers[0] = StringToInt(versionString.Substring(0, i));
                break;
            }
        }


        int substringIndex = -1;
        for (int i = 0; i < versionString.Length; i++)
        {
            if (versionString[i] == '.')
            {
                substringIndex = i + 1;
                break;
            }
        }
        versionNumbers[1] = StringToInt(versionString.Substring(substringIndex));
        return versionNumbers;
    }

    static internal void DrawSpriteBetween2Points(SpriteRenderer a_spriteRenderer, Vector3 a_pointA, Vector3 a_pointB)
    {
        float textureHeight = a_spriteRenderer.sprite.texture.height;
        float pixelsPerUnit = a_spriteRenderer.sprite.pixelsPerUnit;

        Vector3 distanceVector = a_pointB - a_pointA;

        float desiredHeight = distanceVector.magnitude * pixelsPerUnit / textureHeight;
        float currentHeight = a_spriteRenderer.bounds.size.y;
        float scale = desiredHeight / currentHeight;

        a_spriteRenderer.transform.position = a_pointA + distanceVector / 2f;
        a_spriteRenderer.transform.eulerAngles = new Vector3(0f, 0f, VLib.Vector3ToEulerAngle(distanceVector));
        a_spriteRenderer.transform.localScale = new Vector3(a_spriteRenderer.transform.localScale.x, desiredHeight,a_spriteRenderer.transform.localScale.z);
    }

    static internal void DrawTilingSpriteBetween2Points(SpriteRenderer a_spriteRenderer, Vector3 a_pointA, Vector3 a_pointB)
    {
        float textureHeight = a_spriteRenderer.sprite.texture.height;
        float pixelsPerUnit = a_spriteRenderer.sprite.pixelsPerUnit;

        Vector3 distanceVector = a_pointB - a_pointA;
        float distance = distanceVector.magnitude;
        float desiredHeight = distanceVector.magnitude / a_spriteRenderer.transform.localScale.y;
        float currentHeight = a_spriteRenderer.bounds.size.y;
        float scale = desiredHeight / currentHeight;

        a_spriteRenderer.transform.position = a_pointA + distanceVector / 2f;
        a_spriteRenderer.transform.eulerAngles = new Vector3(0f, 0f, VLib.Vector3ToEulerAngle(distanceVector));
        a_spriteRenderer.size = new Vector2(a_spriteRenderer.size.x, desiredHeight);
    }
}