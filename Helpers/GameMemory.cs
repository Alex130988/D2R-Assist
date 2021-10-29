/**
 *   Copyright (C) 2021 okaygo
 *
 *   https://github.com/misterokaygo/MapAssist/
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <https://www.gnu.org/licenses/>.
 **/

using MapAssist.Types;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace MapAssist.Helpers
{
    class GameMemory
    {
        private static string ProcessName = Encoding.UTF8.GetString(new byte[] { 68, 50, 82 });
        private static IntPtr AdrPlayerUnit = IntPtr.Zero;
        private static IntPtr PtrPlayerUnit = IntPtr.Zero;

        public static GameData GetGameData()
        {
            var addressBuffer = new byte[8];
            var dwordBuffer = new byte[4];
            var byteBuffer = new byte[1];
            var stringBuffer = new byte[16];

            IntPtr processHandle = IntPtr.Zero;
            // Clean up and organize, add better exception handeling.
            try
            {
                Process[] process = Process.GetProcessesByName(ProcessName);
                Process gameProcess = process.Length > 0 ? process[0] : null;

                if (gameProcess == null)
                {
                    throw new Exception("Game process not found.");
                }

                processHandle =
                    WindowsExternal.OpenProcess((uint)WindowsExternal.ProcessAccessFlags.VirtualMemoryRead, false, gameProcess.Id);
                IntPtr processAddress = gameProcess.MainModule.BaseAddress;

                if (PtrPlayerUnit == IntPtr.Zero)
                {
                    var pUnitTable = IntPtr.Add(processAddress, Offsets.UnitTable);
                    for (var i = 0; i < 128; i++)
                    {
                        var pUnit = IntPtr.Add(pUnitTable, i * 8);
                        WindowsExternal.ReadProcessMemory(processHandle, pUnit, addressBuffer, addressBuffer.Length, out _);
                        var aUnit = (IntPtr)BitConverter.ToInt64(addressBuffer, 0);

                        if (aUnit != IntPtr.Zero)
                        {
                            var aPlayerUnitCheck = IntPtr.Add(aUnit, 0xB8);
                            WindowsExternal.ReadProcessMemory(processHandle, aPlayerUnitCheck, addressBuffer, addressBuffer.Length, out _);
                            var playerUnitCheck = BitConverter.ToInt64(addressBuffer, 0);
                            if (playerUnitCheck == 0x0000000000000100)
                            {
                                AdrPlayerUnit = aUnit;
                                PtrPlayerUnit = pUnit;
                                break;
                            }
                        }
                    }
                }

                WindowsExternal.ReadProcessMemory(processHandle, PtrPlayerUnit, addressBuffer, addressBuffer.Length, out _);
                AdrPlayerUnit = (IntPtr)BitConverter.ToInt64(addressBuffer, 0);
                var pPlayer = IntPtr.Add(AdrPlayerUnit, 0x10);
                var pAct = IntPtr.Add(AdrPlayerUnit, 0x20);
                var pPath = IntPtr.Add(AdrPlayerUnit, 0x38);

                if (AdrPlayerUnit == IntPtr.Zero)
                {
                    PtrPlayerUnit = IntPtr.Zero;
                    throw new Exception("Player pointer is zero.");
                }

                WindowsExternal.ReadProcessMemory(processHandle, pPlayer, addressBuffer, addressBuffer.Length, out _);
                var aPlayer = (IntPtr)BitConverter.ToInt64(addressBuffer, 0);

                WindowsExternal.ReadProcessMemory(processHandle, aPlayer, stringBuffer, stringBuffer.Length, out _);
                var playerName = Encoding.ASCII.GetString(stringBuffer);

                WindowsExternal.ReadProcessMemory(processHandle, pAct, addressBuffer, addressBuffer.Length, out _);
                var aAct = (IntPtr)BitConverter.ToInt64(addressBuffer, 0);
                var aMapSeed = IntPtr.Add(aAct, 0x14);
                var pActUnk1 = IntPtr.Add(aAct, 0x70);

                WindowsExternal.ReadProcessMemory(processHandle, pActUnk1, addressBuffer, addressBuffer.Length, out _);
                var aActUnk2 = (IntPtr)BitConverter.ToInt64(addressBuffer, 0);
                var aGameDifficulty = IntPtr.Add(aActUnk2, 0x830);

                WindowsExternal.ReadProcessMemory(processHandle, aGameDifficulty, byteBuffer, byteBuffer.Length, out _);
                ushort gameDifficulty = byteBuffer[0];

                // IntPtr aDwAct = IntPtr.Add(aAct, 0x20);
                // WindowsExternal.ReadProcessMemory(processHandle, aDwAct, dwordBuffer, dwordBuffer.Length, out _);

                WindowsExternal.ReadProcessMemory(processHandle, pPath, addressBuffer, addressBuffer.Length, out _);
                var aPath = (IntPtr)BitConverter.ToInt64(addressBuffer, 0);
                var aPositionX = IntPtr.Add(aPath, 0x02);
                var aPositionY = IntPtr.Add(aPath, 0x06);
                var pRoom1 = IntPtr.Add(aPath, 0x20);

                WindowsExternal.ReadProcessMemory(processHandle, aPositionX, addressBuffer, addressBuffer.Length, out _);
                var positionX = BitConverter.ToUInt16(addressBuffer, 0);

                WindowsExternal.ReadProcessMemory(processHandle, aPositionY, addressBuffer, addressBuffer.Length, out _);
                var positionY = BitConverter.ToUInt16(addressBuffer, 0);

                WindowsExternal.ReadProcessMemory(processHandle, pRoom1, addressBuffer, addressBuffer.Length, out _);
                var aRoom1 = (IntPtr)BitConverter.ToInt64(addressBuffer, 0);
                var pRoom2 = IntPtr.Add(aRoom1, 0x18);

                WindowsExternal.ReadProcessMemory(processHandle, pRoom2, addressBuffer, addressBuffer.Length, out _);
                var aRoom2 = (IntPtr)BitConverter.ToInt64(addressBuffer, 0);
                var pLevel = IntPtr.Add(aRoom2, 0x90);

                WindowsExternal.ReadProcessMemory(processHandle, pLevel, addressBuffer, addressBuffer.Length, out _);
                var aLevel = (IntPtr)BitConverter.ToInt64(addressBuffer, 0);
                var aLevelId = IntPtr.Add(aLevel, 0x1F8);

                if (aLevel == IntPtr.Zero)
                {
                    throw new Exception("Level address is zero.");
                }

                WindowsExternal.ReadProcessMemory(processHandle, aLevelId, dwordBuffer, dwordBuffer.Length, out _);
                var levelId = BitConverter.ToUInt32(dwordBuffer, 0);

                WindowsExternal.ReadProcessMemory(processHandle, aMapSeed, dwordBuffer, dwordBuffer.Length, out _);
                var mapSeed = BitConverter.ToUInt32(dwordBuffer, 0);

                var aUiSettingsPath = IntPtr.Add(processAddress, Offsets.InGameMap);
                WindowsExternal.ReadProcessMemory(processHandle, aUiSettingsPath, byteBuffer, byteBuffer.Length, out _);
                var mapShown = BitConverter.ToBoolean(byteBuffer, 0);

                return new GameData
                {
                    PlayerPosition = new Point(positionX, positionY),
                    MapSeed = mapSeed,
                    Area = (Area)levelId,
                    Difficulty = (Difficulty)gameDifficulty,
                    MapShown = mapShown,
                    MainWindowHandle = gameProcess.MainWindowHandle
                };
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (processHandle != IntPtr.Zero)
                {
                    WindowsExternal.CloseHandle(processHandle);
                }
            }
        }
    }
}
