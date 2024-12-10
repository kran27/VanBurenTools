#pragma once
#include <DirectXMath.h>

#include "globals.h"
#include <vector>

#pragma region Internal Functions
typedef void (*WriteToConsoleFunc)(uintptr_t a1, const char *Format, ...);
auto WriteToConsole = reinterpret_cast<WriteToConsoleFunc>(F3 + 0x0019EAF0);

typedef void (*RegisterConsoleCommandFunc)(const char *command, void *function);
auto RegisterConsoleCommand =
    reinterpret_cast<RegisterConsoleCommandFunc>(F3 + 0x0019F5D0);

typedef void (*SendDebugStringFunc)(const char *Format, ...);
auto SendDebugString = reinterpret_cast<SendDebugStringFunc>(F3 + 0x00097120);

typedef void (*sub_46CD60Func)(int a1, const char *Format, ...);
auto WriteToDebugWindow = reinterpret_cast<sub_46CD60Func>(
    F3 + 0x6CD60); // also calls SendDebugString

// sub_41CFE0 (writes to bottom left of screen) // through WriteToFeedback?

typedef uintptr_t (*__cdecl RTDynamicCastFunc)(PVOID inptr, LONG VfDelta,
                                               PVOID SrcType, PVOID TargetType,
                                               BOOL isReference);
auto RTDynamicCast = reinterpret_cast<RTDynamicCastFunc>(F3 + 0x002165E0);
#pragma region Game Types
auto Action = reinterpret_cast<PVOID>(0x6F1E60);
auto Gfx_CamCtrl = reinterpret_cast<PVOID>(0x6FB068);
auto Gfx_CamCtrl_Orbit = reinterpret_cast<PVOID>(0x6FB084);
auto Entity = reinterpret_cast<PVOID>(0x6F1E78);
auto GameCreature = reinterpret_cast<PVOID>(0x6F1E90);
auto GameEntity = reinterpret_cast<PVOID>(0x6F1F24);
auto Effect = reinterpret_cast<PVOID>(0x6F1F40);
auto GameEffectDamage = reinterpret_cast<PVOID>(0x6F1F58);
auto GameActionAttack = reinterpret_cast<PVOID>(0x6F1F78);
auto GameUsableObject = reinterpret_cast<PVOID>(0x6F1F98);
auto GameWeapon = reinterpret_cast<PVOID>(0x6F1FB8);
auto Inventory = reinterpret_cast<PVOID>(0x6F1FD4);
auto GameInventory = reinterpret_cast<PVOID>(0x6F1FEC);
auto GameActionDie = reinterpret_cast<PVOID>(0x6F2070);
auto GameActionEquip = reinterpret_cast<PVOID>(0x6F20A8);
auto GameItem = reinterpret_cast<PVOID>(0x6F20C8);
auto GameDoor = reinterpret_cast<PVOID>(0x6F21CC);
auto Client = reinterpret_cast<PVOID>(0x6F2258);
auto GameClient = reinterpret_cast<PVOID>(0x6F2270);
auto GameContainer = reinterpret_cast<PVOID>(0x6F228C);
auto GameActionPickUp = reinterpret_cast<PVOID>(0x6F22C4);
auto GameAmmo = reinterpret_cast<PVOID>(0x6F2304);
auto GameActionSetActiveWeapon = reinterpret_cast<PVOID>(0x6F2340);
auto GameActionUnequip = reinterpret_cast<PVOID>(0x6F2384);
auto DynamicObject = reinterpret_cast<PVOID>(0x6F2404);
auto IWorld = reinterpret_cast<PVOID>(0x6F2420);
auto GameWorld = reinterpret_cast<PVOID>(0x6F2438);
auto GameArmor = reinterpret_cast<PVOID>(0x6F2450);
auto Server = reinterpret_cast<PVOID>(0x6F24A8);
auto GameServer = reinterpret_cast<PVOID>(0x6F24C0);
auto GamePlayer = reinterpret_cast<PVOID>(0x6F2598);
auto GameProjectile = reinterpret_cast<PVOID>(0x6F25B4);
auto Window = reinterpret_cast<PVOID>(0x6F273C);
auto AreaMapWindow = reinterpret_cast<PVOID>(0x6F2774);
auto Gfx_BasePacketData = reinterpret_cast<PVOID>(0x6F2790);
auto Gfx_PacketData = reinterpret_cast<PVOID>(0x6F27A8);
auto CSMenuInterface = reinterpret_cast<PVOID>(0x6F2D84);
auto GameCSMenuInterface = reinterpret_cast<PVOID>(0x6F2DA4);
auto CSMenuLabel = reinterpret_cast<PVOID>(0x6F2DC8);
auto HotKeys = reinterpret_cast<PVOID>(0x6F31E4);
auto GameHotKeys = reinterpret_cast<PVOID>(0x6F31FC);
auto JournalEntry = reinterpret_cast<PVOID>(0x6F3540);
auto TaskEntry = reinterpret_cast<PVOID>(0x6F355C);
auto Journal = reinterpret_cast<PVOID>(0x6F3574);
auto GameJournal = reinterpret_cast<PVOID>(0x6F358C);
auto Party = reinterpret_cast<PVOID>(0x6F3640);
auto GameParty = reinterpret_cast<PVOID>(0x6F3654);
#pragma endregion

typedef uintptr_t (*__cdecl GetEntityPtrFunc)(int index);
auto GetEntityPtr = reinterpret_cast<GetEntityPtrFunc>(F3 + 0x14BE10);

typedef int (*__cdecl sub_5CCCC0Func)(int a1, int a2, _DWORD *a3);
auto sub_5CCCC0 = reinterpret_cast<sub_5CCCC0Func>(F3 + 0x1CCCC0);

typedef void (*__cdecl sub_59F030Func)(const char *a1);
auto sub_59F030 = reinterpret_cast<sub_59F030Func>(F3 + 0x1F030);

typedef void (*NoParamFunc)();
auto ToggleStatistics = reinterpret_cast<NoParamFunc>(F3 + 0x1882A0);
auto ToggleLighting = reinterpret_cast<NoParamFunc>(F3 + 0x1882C0);
auto ToggleShadow = reinterpret_cast<NoParamFunc>(F3 + 0x1882D0);
auto ToggleCameraFog = reinterpret_cast<NoParamFunc>(F3 + 0x1882F0);
auto ToggleWireframe = reinterpret_cast<NoParamFunc>(F3 + 0x188300);
auto ToggleTransparency = reinterpret_cast<NoParamFunc>(F3 + 0x188310);
auto ToggleOctreeDrawing = reinterpret_cast<NoParamFunc>(F3 + 0x188600);
auto DebugLight = reinterpret_cast<NoParamFunc>(F3 + 0x188320);
auto ToggleFogOfWar = reinterpret_cast<NoParamFunc>(F3 + 0x18B290);
auto ToggleFOWAutomap = reinterpret_cast<NoParamFunc>(F3 + 0x18B370);
auto ToggleFOWLOS = reinterpret_cast<NoParamFunc>(F3 + 0x18E340);
auto ToggleVisualEffects = reinterpret_cast<NoParamFunc>(F3 + 0x18E380);
auto ToggleWater = reinterpret_cast<NoParamFunc>(F3 + 0x18E7E0);
auto CameraToggleConstraints = reinterpret_cast<NoParamFunc>(F3 + 0x1885E0);
auto DebugWindow = reinterpret_cast<NoParamFunc>(F3 + 0x188690);
auto StackWindow = reinterpret_cast<NoParamFunc>(F3 + 0x1886F0);
auto DebugTools = reinterpret_cast<NoParamFunc>(F3 + 0x188750);
auto ToggleScene = reinterpret_cast<NoParamFunc>(F3 + 0x188820);
auto ToggleEntities = reinterpret_cast<NoParamFunc>(F3 + 0x188840);
auto ToggleGUI = reinterpret_cast<NoParamFunc>(F3 + 0x188860);
auto ToggleRain = reinterpret_cast<NoParamFunc>(F3 + 0x606C0);
auto ToggleSnow = reinterpret_cast<NoParamFunc>(F3 + 0x606D0);
auto ToggleWind = reinterpret_cast<NoParamFunc>(F3 + 0x606E0);
#pragma endregion

inline auto getClient() {
  return *reinterpret_cast<int **>(GetAddr(HudBase, {0x2C}));
}
inline auto getServer() {
  return *reinterpret_cast<int **>(GetAddr(HudBase, {0x18}));
} // TODO: probably fails in multiplayer?
inline uintptr_t consolePtr() { return GetAddr(HudBase, {0x2C, 0x88, 0x0}); }
inline uintptr_t cursorPtr() { return GetAddr(HudBase, {0x2C, 0xA4, 0x0}); }

inline int isValid(const char *str) {
  if (!str)
    return 0;
  for (int i = 0; str[i]; ++i) {
    const int c = str[i];
    if (!isascii(c) ||
        !isalnum(c) && c != '-' && c != '_' && c != '.' && c != ' ') {
      return 0;
    }
  }
  return 1;
}

const char *formatString(const char *format, ...) {
  static char buffer[256]; // Adjust the size as per your requirements

  va_list args;
  va_start(args, format);
  vsnprintf(buffer, sizeof(buffer), format, args);
  va_end(args);

  return buffer;
}

inline auto getCurrentEntityPtr() {
  auto client = getClient();
  auto curEntity = client[0x37];
  if (curEntity)
    return GetEntityPtr(curEntity);
}

inline auto getCurrentEntityID() {
  auto client = getClient();
  auto curEntity = client[0x37];
  if (curEntity)
    return curEntity;
}

inline auto ReadString(uintptr_t base) {
  const char *result = nullptr;
  auto str = reinterpret_cast<char const *>(GetAddr(base, {}));
  if (str && isValid(str)) {
    result = str;
  } else {
    str = reinterpret_cast<char const *>(GetAddr(base, {0x0}));
    if (str && isValid(str)) {
      result = str;
    } else {
      result = "";
    }
  }
  return result;
}

inline auto ReadString(uintptr_t base, std::vector<uintptr_t> offsets) {
  const char *result = nullptr;
  auto str = reinterpret_cast<char const *>(GetAddr(base, offsets));
  if (str && isValid(str)) {
    result = str;
  } else {
    offsets.insert(offsets.end(), 0x0);
    str = reinterpret_cast<char const *>(GetAddr(base, offsets));
    if (str && isValid(str)) {
      result = str;
    } else {
      result = "";
    }
  }
  return result;
}

inline auto mapName() { return ReadString(SettingsBase, {0xFC}); }

inline float *getCamPtr() {
  const auto ptr = reinterpret_cast<char *>(GetAddr(SettingsBase, {0x0}));
  return (float *)(*(int(__thiscall **)(char *))(*(_DWORD *)ptr + 0xA8))(ptr);
}

inline float *getCamPtr2() {
  const auto ptr = reinterpret_cast<char *>(GetAddr(SettingsBase, {0x0}));
  const auto v2 =
      (float *)(*(int(__thiscall **)(char *))(*(_DWORD *)ptr + 0xAC))(ptr);
  const auto v11 = (float *)RTDynamicCast((_DWORD *)v2, NULL, Gfx_CamCtrl,
                                          Gfx_CamCtrl_Orbit, NULL);
  return v11;
}

inline uintptr_t getPlayerptr() {
  int *playerPtr = nullptr;

  auto dwd = Read<_DWORD *>(dword_70BE0C);
  auto hb = Read<_DWORD>(HudBase);

  _DWORD *v5 = dwd ? *(_DWORD **)dwd : nullptr;

  if (v5 != dwd) {
    do {
      auto result = v5[2];
      auto v6 = *(_DWORD *)(result + 0x1E4);
      if (v6) {
        auto v7 = *(_DWORD *)(hb + 0x2C);
        if (v6 == *(_DWORD *)(v7 + 0xBC)) {
          playerPtr = (int *)v5[2];
          result = *(_DWORD *)(result + 0x1DC);
          if (result == *(_DWORD *)(v7 + 0xE0))
            break;
        }
      }
      v5 = (_DWORD *)*v5;
    } while (v5 != dwd);
    if (playerPtr)
      return (uintptr_t)playerPtr;
  }
  return 0;
}

// to be used in a custom console command, a1 as the value given to the
// function, count as the number of params needed.
inline auto getParams(unsigned a1, unsigned count) {
  std::vector<const char *> values;
  values.reserve(count); // Reserve space in the vector
  auto v1 = *(int *)(a1 + 4);
  for (unsigned i = 1; i <= count; ++i) {
    auto value = reinterpret_cast<const char *>(*(int *)(v1 + i * 4));
    values.emplace_back(value); // Use emplace_back instead of push_back
  }
  return values;
}

// sends a given string to the in-game console, debug console, and debug log.
inline void DebugAndConsole(const char *format, ...) {
  va_list args;
  va_start(args, format);
  char result[1024];
  vsprintf_s(result, format, args);
  va_end(args);
  strcat_s(result, "\n");
  WriteToConsole(consolePtr(), result);
  SendDebugString(result);
}

inline const char *GetHealthTxt() {
  auto cur = getCurrentEntityPtr();
  auto ent = (int)RTDynamicCast((uint32 *)cur, NULL, Entity, GameEntity, NULL);
  auto mhp = Read<int>(ent + 0x20);
  auto hp = Read<int>(ent + 0x24);

  static char healthTxt[256];
  sprintf_s(healthTxt, "%d/%d", hp, mhp);
  return healthTxt;
}

inline const char *GetMouseOverText() {
  auto text = "";
  auto client = getClient();

  if (client[0x37]) {
    auto entPtr = GetEntityPtr(client[0x37]);
    text = ReadString(entPtr + (useNames ? 0x1C4 : 0x71));
    static char mouseOverText[512];
    sprintf_s(mouseOverText, "%s\n%s", text, GetHealthTxt());
    return mouseOverText;
  }
  text = ReadString(F3 + 0x307EBC);
  return text;
}

int GetDerivedAttribute(_DWORD *entity, int attribute) {
  unsigned __int64 v2; // rax
  double v3;           // st7

  switch (attribute) {
  case 0:
    v2 = entity[477] + 20;
    break;
  case 1:
    v2 = 25 * (entity[472] + 1);
    break;
  case 2:
    v2 = 2 * entity[473];
    break;
  case 3:
    v2 = entity[478];
    break;
  case 4:
    v2 = entity[511];
    break;
  case 5:
    v2 = entity[474];
    break;
  case 6:
    v2 = entity[472];
    break;
  case 7:
    v2 = 3 * entity[473] + 40;
    break;
  case 8:
    v2 = 3;
    break;
  case 9:
  case 10:
  case 11:
    goto LABEL_17;
  case 12:
    v2 = 2 * entity[476] + 5;
    break;
  case 13:
    v2 = 3 * entity[472];
    break;
  case 14:
    v3 = (double)GetDerivedAttribute(entity, 0) * 0.0001666666666666667;
    if (v3 == 0.0)
      v2 = 1000;
    else
      v2 = (unsigned __int64)(1.0 / v3);
    break;
  default:
    SendDebugString(
        "GameCreature::GetDerivedAttribute() - Unknown attribute requested: %d",
        attribute);
  LABEL_17:
    v2 = 0;
    break;
  }
  return v2;
}

// Injected Functions

inline void HealthTest(const int a1) {
  auto param = getParams(a1, 1)[0];
  auto health = atoi(param);
  auto cur = getCurrentEntityPtr();
  auto ent = (int)RTDynamicCast((uint32 *)cur, NULL, Entity, GameEntity, NULL);
  Write<int>(ent + 0x24, health);
}

inline auto EntityInfo() {
  const char *entPtr;   // edi
  const char *name;     // eax
  const char *script;   // eax
  const char *dialogue; // eax
  const char *filename; // eax
  const char *icon;     // eax
  int hp;               // edi
  int maxhp;            // eax
  int gentPtr;          // [esp+28h] [ebp-134h]

  entPtr = (const char *)getCurrentEntityPtr();
  gentPtr = (int)RTDynamicCast((PVOID)entPtr, 0, Entity, GameEntity, 0);
  if (!entPtr)
    return 0;
  DebugAndConsole("**Entity Info**");
  if (*((_DWORD *)entPtr + 0x76) < 0x10u)
    name = entPtr + 0x1C4;
  else
    name = (const char *)*((_DWORD *)entPtr + 0x71);
  DebugAndConsole("Instance Name: %s", name);
  DebugAndConsole("Entity ID #: %d", *((_DWORD *)entPtr + 0x77));
  if (*((_DWORD *)entPtr + 0xAE) < 0x10u)
    script = entPtr + 0x2A4;
  else
    script = (const char *)*((_DWORD *)entPtr + 0xA9);
  DebugAndConsole("Script File name: %s", script);
  DebugAndConsole("Team ID and Rank: %d and %d", *((_DWORD *)entPtr + 0xDA),
                  *((_DWORD *)entPtr + 0xDB));
  DebugAndConsole("Squad ID and Rank: %d and %d", *((_DWORD *)entPtr + 0xDC),
                  *((_DWORD *)entPtr + 0xDD));
  if (*((_DWORD *)entPtr + 0x85) < 0x10u)
    dialogue = entPtr + 0x200;
  else
    dialogue = (const char *)*((_DWORD *)entPtr + 0x80);
  DebugAndConsole("Dialogue File Name: %s", dialogue);
  if (*((_DWORD *)entPtr + 0x69) < 0x10u)
    filename = entPtr + 0x190;
  else
    filename = (const char *)*((_DWORD *)entPtr + 0x64);
  DebugAndConsole("Entity File Name: %s", filename);
  if (*((_DWORD *)entPtr + 0x9A) < 0x10u)
    icon = entPtr + 0x254;
  else
    icon = (const char *)*((_DWORD *)entPtr + 0x95);
  DebugAndConsole("Entity Icon File: %s", icon);
  DebugAndConsole("Current Map ID: %p", *((_DWORD *)entPtr + 0x7B));
  DebugAndConsole("Owner's ID: %d", *((_DWORD *)entPtr + 0xB7));
  if (*((_DWORD *)entPtr + 0x79))
    DebugAndConsole("This entity is client controlled.");
  else
    DebugAndConsole("This entity is not client controlled.");
  if (gentPtr) {
    DebugAndConsole("**GameEntity Level Info**");
    hp = *(_DWORD *)(gentPtr + 0x24);
    maxhp = *(_DWORD *)(gentPtr + 0x20);
    DebugAndConsole("Current Hitpoints %d/%d", hp, maxhp);
  }
  DebugAndConsole("**End of Entity Info**");
  return 1;
}

std::vector<float> worldToScreen(const float x, const float y, const float z) {
  const auto camPtr = getCamPtr();
  const auto camPtr2 = getCamPtr2();

  DirectX::XMFLOAT3 cameraPos = DirectX::XMFLOAT3(
      Read<float>(SettingsBase + 0xFC), Read<float>(SettingsBase + 0x100),
      Read<float>(SettingsBase + 0x104));
  DirectX::XMFLOAT3 cameraTarget =
      DirectX::XMFLOAT3(camPtr2[36], camPtr2[37], camPtr2[38]);
  float fov = camPtr[68];
  float aspectRatio =
      static_cast<float>(Read<int>(F3 + 0x307EA8)) / Read<int>(F3 + 0x307EAC);

  DirectX::XMFLOAT3 cameraUp = DirectX::XMFLOAT3(0.0f, 1.0f, 0.0f);
  float nearPlane = 1.0f;
  float farPlane = 10000.0f;

  // ensure pos and target are not the same
  if (cameraPos.x == cameraTarget.x && cameraPos.y == cameraTarget.y &&
      cameraPos.z == cameraTarget.z)
    cameraPos.z += 0.01f;
  DirectX::XMMATRIX viewMatrix = DirectX::XMMatrixLookAtLH(
      DirectX::XMLoadFloat3(&cameraPos), XMLoadFloat3(&cameraTarget),
      XMLoadFloat3(&cameraUp));
  DirectX::XMMATRIX projectionMatrix =
      DirectX::XMMatrixPerspectiveFovLH(fov, aspectRatio, nearPlane, farPlane);
  DirectX::XMMATRIX viewProjectionMatrix = viewMatrix * projectionMatrix;
  DirectX::XMFLOAT3 worldPos = DirectX::XMFLOAT3(x, z, y);

  DirectX::XMVECTOR clipSpacePos =
      XMVector3TransformCoord(XMLoadFloat3(&worldPos), viewProjectionMatrix);
  DirectX::XMVECTOR ndcPos = DirectX::XMVectorSetW(
      clipSpacePos, 1.0f / DirectX::XMVectorGetW(clipSpacePos));
  D3D11_VIEWPORT viewport = {
      0.0f, 0.0f, Read<int>(F3 + 0x307EA8), Read<int>(F3 + 0x307EAC),
      0.0f, 1.0f};
  DirectX::XMMATRIX viewportMatrix =
      DirectX::XMMatrixScaling(viewport.Width / 2.0f, -viewport.Height / 2.0f,
                               1.0f) *
      DirectX::XMMatrixTranslation(viewport.Width / 2.0f + viewport.TopLeftX,
                                   viewport.Height / 2.0f + viewport.TopLeftY,
                                   0.0f);

  DirectX::XMVECTOR screenPos = XMVector3TransformCoord(ndcPos, viewportMatrix);

  std::vector a = {DirectX::XMVectorGetX(screenPos),
                   DirectX::XMVectorGetY(screenPos)};
  return a;
}

inline void GetVBEVersion() {
  WriteToConsole(consolePtr(), "Van Buren Extender is %s", version);
}

static bool showConsole = false;
inline void ToggleConsole() { showConsole = !showConsole; }

inline void TestFunction(int a1) {
  auto params = getParams(a1, 1);
  WriteToDebugWindow(0, "TestFunction(%d)", params[0]);
}