#pragma once
static auto F3 = reinterpret_cast<uintptr_t>(GetModuleHandleW(nullptr));

constexpr auto version = "v0.1";

static bool useNames = false;

static int selEntity = 0;
const char *entities[64] = {""};

float entPos[3];

int AP;

int MaxHP;
int HP;
int Special[7];
int Age;
int Level;
char Locked;
int Race;
int Gender;
int Ethnicity;
int SubRace;
int Type;
int LockDC;
int Active;

constexpr auto RadToDeg = 57.295776f;

const auto EntityBase = F3 + 0x00307F08;
const auto HudBase = F3 + 0x0030BD90;
const auto SettingsBase = F3 + 0x00307CE4;
const auto dword_70BE0C = F3 + 0x0030BE0C;