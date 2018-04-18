using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.IO;


public class UIWordUnitsManager : BaseSingleton<UIWordUnitsManager>
{
    public enum LanguageType : byte
    {
        LanguageType_English,
        LanguageType_TraditionalChinese,
        LanguageType_Japanese,
        LanguageType_SimplifiedChinese,
        LanguageType_Max,
    }

    // default language - english
    private LanguageType languageType = LanguageType.LanguageType_English;
    public UIWordData UIWordData;
    public Action onChangeLanguage;


    /* Constructor */
    UIWordUnitsManager()
    {
    }


    public override void init()
    {
        UIWordData = new UIWordData();
        UIWordData.loadStringTable(languageType);
    }

    public void changeGameLanguage(LanguageType languageType)
    {
        this.languageType = languageType;
        UIWordData.loadStringTable(languageType);

        if (onChangeLanguage != null)
            onChangeLanguage();
    }
}


public class UIWordData
{
    private string[] stringTableData;
    private int recordAmount;
    private int maxKey;
    private IntPtr bytesIntPtr_Key;
    private IntPtr bytesIntPtr;

    public UIWordData()
    {
        bytesIntPtr_Key = IntPtr.Zero;
        bytesIntPtr = IntPtr.Zero;
    }

    ~UIWordData()
    {
        clearStringTable();
        if (bytesIntPtr_Key != IntPtr.Zero)
        {
            GCHandle gch = GCHandle.FromIntPtr(bytesIntPtr_Key);
            if (gch.IsAllocated)
                gch.Free();
        }
        if (bytesIntPtr != IntPtr.Zero)
        {
            GCHandle gch = GCHandle.FromIntPtr(bytesIntPtr);
            if (gch.IsAllocated)
                gch.Free();
        }
    }

    public bool loadStringTable(UIWordUnitsManager.LanguageType language)
    {
        TextAsset asset;
        TextAsset asset2;
        switch (language)
        {
            default:
            case UIWordUnitsManager.LanguageType.LanguageType_English:
                //asset = AssetBundleLoader.getInstance().loadBytesSync("Data/UIWord_EN");
                //asset2 = AssetBundleLoader.getInstance().loadBytesSync("Data/UIWord2_EN");
                asset = Resources.Load<TextAsset>("Data/UIWord_EN");
                asset2 = Resources.Load<TextAsset>("Data/UIWord2_EN");
                break;
            case UIWordUnitsManager.LanguageType.LanguageType_TraditionalChinese:
                //asset = AssetBundleLoader.getInstance().loadBytesSync("Data/UIWord_TW");
                //asset2 = AssetBundleLoader.getInstance().loadBytesSync("Data/UIWord2_TW");
                asset = Resources.Load<TextAsset>("Data/UIWord_TW");
                asset2 = Resources.Load<TextAsset>("Data/UIWord2_TW");
                break;
        }

        if (asset == null || asset2 == null)
        {
            //EfficacyDebuger.logConsole("StringTable::LoadStringTable Resources.Load error");
            return false;
        }

        recordAmount = 0;

        Stream s2 = new MemoryStream(asset2.bytes);
        using (BinaryReader br = new BinaryReader(s2))
        {
            recordAmount = br.ReadInt32();
            GCHandle handle2 = GCHandle.Alloc(asset2.bytes, GCHandleType.Pinned);
            bytesIntPtr_Key = handle2.AddrOfPinnedObject();
        }
        s2.Close();
        GCHandle handle;// = GCHandle.Alloc(asset.bytes, GCHandleType.Pinned);
        Stream s1 = new MemoryStream(asset.bytes);
        using (BinaryReader br = new BinaryReader(s1))
        {
            maxKey = br.ReadInt32();
            handle = GCHandle.Alloc(asset.bytes, GCHandleType.Pinned);
            bytesIntPtr = handle.AddrOfPinnedObject();

            int count = (maxKey / 8) + 1;
            stringTableData = new string[count];

            ushort Idx = 0;
            int offset = 4;
            int MaxAmount = (recordAmount / 2) + 1;
            for (int i = 1; i < MaxAmount; i++)
            {
                offset = 4 + (int)i * 2;
                IntPtr pRptr;

                unsafe
                {
                    byte* pBuffer = (byte*)bytesIntPtr_Key.ToPointer();
                    pBuffer += offset;
                    pRptr = new IntPtr(pBuffer);
                    Idx = (ushort)Marshal.PtrToStructure(pRptr, typeof(ushort));
                    if (Idx > 0 && Idx < count && stringTableData[Idx] == null)
                    {
                        pBuffer = (byte*)bytesIntPtr.ToPointer();
                        offset = 4 + (Idx - 1) * 8;
                        pBuffer += offset;
                        pRptr = new IntPtr(pBuffer);
                        int Begin = (int)Marshal.PtrToStructure(pRptr, typeof(int));
                        pBuffer += 4;
                        pRptr = new IntPtr(pBuffer);
                        ushort Lenght = (ushort)Marshal.PtrToStructure(pRptr, typeof(ushort));
                        offset = 4 + maxKey + Begin - 32;
                        pBuffer = (byte*)bytesIntPtr.ToPointer();
                        pBuffer += offset;
                        pRptr = new IntPtr(pBuffer);
                        stringTableData[Idx] = new string((sbyte*)pRptr, 0, Lenght, System.Text.Encoding.UTF8);
                    }
                }
            }
            br.Close();
        }
        s1.Close();
        if (handle.IsAllocated == true)
            handle.Free();

        return true;
    }

    public bool getStringByID(int uiWordID, ref string str)
    {
        ushort index = 0;
        int offset = 4;
        if (uiWordID > 0 && uiWordID <= recordAmount / 2)
        {
            offset += (int)uiWordID * 2;
            IntPtr Rptr;

            unsafe
            {
                byte* buffer = (byte*)bytesIntPtr_Key.ToPointer();
                buffer += offset;
                Rptr = new IntPtr(buffer);
                index = (ushort)Marshal.PtrToStructure(Rptr, typeof(ushort));
            }
        }
        else
        {
            str = "";
            return false;
        }

        if (stringTableData[index] == null)
        {
            str = "";
            return false;
        }
        else
        {
            str = stringTableData[index];
            return true;
        }
    }

    public void clearStringTable()
    {
        if (stringTableData != null)
            Array.Clear(stringTableData, 0, stringTableData.Length);

        stringTableData = null;
    }
}
