using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputSpritesByKey : MonoBehaviour
{
    [Header("TASTIERA - LETTERE")]
    [SerializeField] private Sprite spriteA;
    [SerializeField] private Sprite spriteB;
    [SerializeField] private Sprite spriteC;
    [SerializeField] private Sprite spriteD;
    [SerializeField] private Sprite spriteE;
    [SerializeField] private Sprite spriteF;
    [SerializeField] private Sprite spriteG;
    [SerializeField] private Sprite spriteH;
    [SerializeField] private Sprite spriteI;
    [SerializeField] private Sprite spriteJ;
    [SerializeField] private Sprite spriteK;
    [SerializeField] private Sprite spriteL;
    [SerializeField] private Sprite spriteM;
    [SerializeField] private Sprite spriteN;
    [SerializeField] private Sprite spriteO;
    [SerializeField] private Sprite spriteP;
    [SerializeField] private Sprite spriteQ;
    [SerializeField] private Sprite spriteR;
    [SerializeField] private Sprite spriteS;
    [SerializeField] private Sprite spriteT;
    [SerializeField] private Sprite spriteU;
    [SerializeField] private Sprite spriteV;
    [SerializeField] private Sprite spriteW;
    [SerializeField] private Sprite spriteX;
    [SerializeField] private Sprite spriteY;
    [SerializeField] private Sprite spriteZ;

    [Header("TASTIERA - NUMERI TOP")]
    [SerializeField] private Sprite sprite0;
    [SerializeField] private Sprite sprite1;
    [SerializeField] private Sprite sprite2;
    [SerializeField] private Sprite sprite3;
    [SerializeField] private Sprite sprite4;
    [SerializeField] private Sprite sprite5;
    [SerializeField] private Sprite sprite6;
    [SerializeField] private Sprite sprite7;
    [SerializeField] private Sprite sprite8;
    [SerializeField] private Sprite sprite9;

    [Header("TASTIERA - NUMPAD")]
    [SerializeField] private Sprite spriteNumpad0;
    [SerializeField] private Sprite spriteNumpad1;
    [SerializeField] private Sprite spriteNumpad2;
    [SerializeField] private Sprite spriteNumpad3;
    [SerializeField] private Sprite spriteNumpad4;
    [SerializeField] private Sprite spriteNumpad5;
    [SerializeField] private Sprite spriteNumpad6;
    [SerializeField] private Sprite spriteNumpad7;
    [SerializeField] private Sprite spriteNumpad8;
    [SerializeField] private Sprite spriteNumpad9;
    [SerializeField] private Sprite spriteNumpadPlus;
    [SerializeField] private Sprite spriteNumpadMinus;
    [SerializeField] private Sprite spriteNumpadMultiply;
    [SerializeField] private Sprite spriteNumpadDivide;
    [SerializeField] private Sprite spriteNumpadDot;
    [SerializeField] private Sprite spriteNumpadEnter;
    [SerializeField] private Sprite spriteNumLock;

    [Header("TASTIERA - FRECCE")]
    [SerializeField] private Sprite spriteUpArrow;
    [SerializeField] private Sprite spriteDownArrow;
    [SerializeField] private Sprite spriteLeftArrow;
    [SerializeField] private Sprite spriteRightArrow;

    [Header("TASTIERA - MODIFIER")]
    [SerializeField] private Sprite spriteLeftShift;
    [SerializeField] private Sprite spriteRightShift;
    [SerializeField] private Sprite spriteLeftCtrl;
    [SerializeField] private Sprite spriteRightCtrl;
    [SerializeField] private Sprite spriteLeftAlt;
    [SerializeField] private Sprite spriteRightAlt;
    [SerializeField] private Sprite spriteCapsLock;
    [SerializeField] private Sprite spriteTab;
    [SerializeField] private Sprite spriteSpace;
    [SerializeField] private Sprite spriteEnter;
    [SerializeField] private Sprite spriteBackspace;
    [SerializeField] private Sprite spriteEscape;

    [Header("TASTIERA - SPECIALI VARI")]
    [SerializeField] private Sprite spriteInsert;
    [SerializeField] private Sprite spriteDelete;
    [SerializeField] private Sprite spriteHome;
    [SerializeField] private Sprite spriteEnd;
    [SerializeField] private Sprite spritePageUp;
    [SerializeField] private Sprite spritePageDown;
    [SerializeField] private Sprite spritePause;
    [SerializeField] private Sprite spriteScrollLock;
    [SerializeField] private Sprite spritePrintScreen;

    [Header("TASTIERA - PUNTEGGIATURA")]
    [SerializeField] private Sprite spriteMinus;       // -
    [SerializeField] private Sprite spriteEquals;      // =
    [SerializeField] private Sprite spriteLeftBracket; // [
    [SerializeField] private Sprite spriteRightBracket;// ]
    [SerializeField] private Sprite spriteBackslash;   // \
    [SerializeField] private Sprite spriteSemicolon;   // ;
    [SerializeField] private Sprite spriteQuote;       // '
    [SerializeField] private Sprite spriteComma;       // ,
    [SerializeField] private Sprite spritePeriod;      // .
    [SerializeField] private Sprite spriteSlash;       // /

    [Header("TASTIERA - FUNZIONI")]
    [SerializeField] private Sprite spriteF1;
    [SerializeField] private Sprite spriteF2;
    [SerializeField] private Sprite spriteF3;
    [SerializeField] private Sprite spriteF4;
    [SerializeField] private Sprite spriteF5;
    [SerializeField] private Sprite spriteF6;
    [SerializeField] private Sprite spriteF7;
    [SerializeField] private Sprite spriteF8;
    [SerializeField] private Sprite spriteF9;
    [SerializeField] private Sprite spriteF10;
    [SerializeField] private Sprite spriteF11;
    [SerializeField] private Sprite spriteF12;

    [Header("MOUSE")]
    [SerializeField] private Sprite spriteMouseLeft;
    [SerializeField] private Sprite spriteMouseRight;
    [SerializeField] private Sprite spriteMouseMiddle;

    [Header("GAMEPAD - BUTTONI PRINCIPALI")]
    [SerializeField] private Sprite spriteGamepadButtonSouth;      // A / Cross
    [SerializeField] private Sprite spriteGamepadButtonNorth;      // Y / Triangle
    [SerializeField] private Sprite spriteGamepadButtonWest;       // X / Square
    [SerializeField] private Sprite spriteGamepadButtonEast;       // B / Circle
    [SerializeField] private Sprite spriteGamepadLeftBumper;
    [SerializeField] private Sprite spriteGamepadRightBumper;
    [SerializeField] private Sprite spriteGamepadLeftTrigger;
    [SerializeField] private Sprite spriteGamepadRightTrigger;
    [SerializeField] private Sprite spriteGamepadSelect;           // View/Back/Share
    [SerializeField] private Sprite spriteGamepadStart;            // Menu/Options
    [SerializeField] private Sprite spriteGamepadLeftStickClick;
    [SerializeField] private Sprite spriteGamepadRightStickClick;
    [SerializeField] private Sprite spriteGamepadDpadUp;
    [SerializeField] private Sprite spriteGamepadDpadDown;
    [SerializeField] private Sprite spriteGamepadDpadLeft;
    [SerializeField] private Sprite spriteGamepadDpadRight;

    [Header("DEFAULT")]
    [SerializeField] private Sprite defaultSprite;

    public Sprite GetSpriteFromBindingPath(string bindingPath)
    {
        switch (bindingPath)
        {
            // LETTERE
            case "<Keyboard>/a": return spriteA;
            case "<Keyboard>/b": return spriteB;
            case "<Keyboard>/c": return spriteC;
            case "<Keyboard>/d": return spriteD;
            case "<Keyboard>/e": return spriteE;
            case "<Keyboard>/f": return spriteF;
            case "<Keyboard>/g": return spriteG;
            case "<Keyboard>/h": return spriteH;
            case "<Keyboard>/i": return spriteI;
            case "<Keyboard>/j": return spriteJ;
            case "<Keyboard>/k": return spriteK;
            case "<Keyboard>/l": return spriteL;
            case "<Keyboard>/m": return spriteM;
            case "<Keyboard>/n": return spriteN;
            case "<Keyboard>/o": return spriteO;
            case "<Keyboard>/p": return spriteP;
            case "<Keyboard>/q": return spriteQ;
            case "<Keyboard>/r": return spriteR;
            case "<Keyboard>/s": return spriteS;
            case "<Keyboard>/t": return spriteT;
            case "<Keyboard>/u": return spriteU;
            case "<Keyboard>/v": return spriteV;
            case "<Keyboard>/w": return spriteW;
            case "<Keyboard>/x": return spriteX;
            case "<Keyboard>/y": return spriteY;
            case "<Keyboard>/z": return spriteZ;

            // NUMERI TOP
            case "<Keyboard>/0": return sprite0;
            case "<Keyboard>/1": return sprite1;
            case "<Keyboard>/2": return sprite2;
            case "<Keyboard>/3": return sprite3;
            case "<Keyboard>/4": return sprite4;
            case "<Keyboard>/5": return sprite5;
            case "<Keyboard>/6": return sprite6;
            case "<Keyboard>/7": return sprite7;
            case "<Keyboard>/8": return sprite8;
            case "<Keyboard>/9": return sprite9;

            // NUMPAD
            case "<Keyboard>/numpad0": return spriteNumpad0;
            case "<Keyboard>/numpad1": return spriteNumpad1;
            case "<Keyboard>/numpad2": return spriteNumpad2;
            case "<Keyboard>/numpad3": return spriteNumpad3;
            case "<Keyboard>/numpad4": return spriteNumpad4;
            case "<Keyboard>/numpad5": return spriteNumpad5;
            case "<Keyboard>/numpad6": return spriteNumpad6;
            case "<Keyboard>/numpad7": return spriteNumpad7;
            case "<Keyboard>/numpad8": return spriteNumpad8;
            case "<Keyboard>/numpad9": return spriteNumpad9;
            case "<Keyboard>/numpadPlus": return spriteNumpadPlus;
            case "<Keyboard>/numpadMinus": return spriteNumpadMinus;
            case "<Keyboard>/numpadMultiply": return spriteNumpadMultiply;
            case "<Keyboard>/numpadDivide": return spriteNumpadDivide;
            case "<Keyboard>/numpadDecimal": return spriteNumpadDot;
            case "<Keyboard>/numpadEnter": return spriteNumpadEnter;
            case "<Keyboard>/numLock": return spriteNumLock;

            // FRECCE
            case "<Keyboard>/upArrow": return spriteUpArrow;
            case "<Keyboard>/downArrow": return spriteDownArrow;
            case "<Keyboard>/leftArrow": return spriteLeftArrow;
            case "<Keyboard>/rightArrow": return spriteRightArrow;

            // MODIFIER
            case "<Keyboard>/leftShift": return spriteLeftShift;
            case "<Keyboard>/rightShift": return spriteRightShift;
            case "<Keyboard>/leftCtrl": return spriteLeftCtrl;
            case "<Keyboard>/rightCtrl": return spriteRightCtrl;
            case "<Keyboard>/leftAlt": return spriteLeftAlt;
            case "<Keyboard>/rightAlt": return spriteRightAlt;
            case "<Keyboard>/capsLock": return spriteCapsLock;
            case "<Keyboard>/tab": return spriteTab;
            case "<Keyboard>/space": return spriteSpace;
            case "<Keyboard>/enter": return spriteEnter;
            case "<Keyboard>/backspace": return spriteBackspace;
            case "<Keyboard>/escape": return spriteEscape;

            // SPECIALI VARI
            case "<Keyboard>/insert": return spriteInsert;
            case "<Keyboard>/delete": return spriteDelete;
            case "<Keyboard>/home": return spriteHome;
            case "<Keyboard>/end": return spriteEnd;
            case "<Keyboard>/pageUp": return spritePageUp;
            case "<Keyboard>/pageDown": return spritePageDown;
            case "<Keyboard>/pause": return spritePause;
            case "<Keyboard>/scrollLock": return spriteScrollLock;
            case "<Keyboard>/printScreen": return spritePrintScreen;

            // PUNTEGGIATURA
            case "<Keyboard>/minus": return spriteMinus;
            case "<Keyboard>/equals": return spriteEquals;
            case "<Keyboard>/leftBracket": return spriteLeftBracket;
            case "<Keyboard>/rightBracket": return spriteRightBracket;
            case "<Keyboard>/backslash": return spriteBackslash;
            case "<Keyboard>/semicolon": return spriteSemicolon;
            case "<Keyboard>/quote": return spriteQuote;
            case "<Keyboard>/comma": return spriteComma;
            case "<Keyboard>/period": return spritePeriod;
            case "<Keyboard>/slash": return spriteSlash;

            // FUNZIONI
            case "<Keyboard>/f1": return spriteF1;
            case "<Keyboard>/f2": return spriteF2;
            case "<Keyboard>/f3": return spriteF3;
            case "<Keyboard>/f4": return spriteF4;
            case "<Keyboard>/f5": return spriteF5;
            case "<Keyboard>/f6": return spriteF6;
            case "<Keyboard>/f7": return spriteF7;
            case "<Keyboard>/f8": return spriteF8;
            case "<Keyboard>/f9": return spriteF9;
            case "<Keyboard>/f10": return spriteF10;
            case "<Keyboard>/f11": return spriteF11;
            case "<Keyboard>/f12": return spriteF12;

            // MOUSE
            case "<Mouse>/leftButton": return spriteMouseLeft;
            case "<Mouse>/rightButton": return spriteMouseRight;
            case "<Mouse>/middleButton": return spriteMouseMiddle;

            // GAMEPAD
            case "<Gamepad>/buttonSouth": return spriteGamepadButtonSouth;
            case "<Gamepad>/buttonNorth": return spriteGamepadButtonNorth;
            case "<Gamepad>/buttonWest": return spriteGamepadButtonWest;
            case "<Gamepad>/buttonEast": return spriteGamepadButtonEast;
            case "<Gamepad>/leftShoulder": return spriteGamepadLeftBumper;
            case "<Gamepad>/rightShoulder": return spriteGamepadRightBumper;
            case "<Gamepad>/leftTrigger": return spriteGamepadLeftTrigger;
            case "<Gamepad>/rightTrigger": return spriteGamepadRightTrigger;
            case "<Gamepad>/select": return spriteGamepadSelect;
            case "<Gamepad>/start": return spriteGamepadStart;
            case "<Gamepad>/leftStickPress": return spriteGamepadLeftStickClick;
            case "<Gamepad>/rightStickPress": return spriteGamepadRightStickClick;
            case "<Gamepad>/dpad/up": return spriteGamepadDpadUp;
            case "<Gamepad>/dpad/down": return spriteGamepadDpadDown;
            case "<Gamepad>/dpad/left": return spriteGamepadDpadLeft;
            case "<Gamepad>/dpad/right": return spriteGamepadDpadRight;

            default:
                return defaultSprite;
        }
    }
}

