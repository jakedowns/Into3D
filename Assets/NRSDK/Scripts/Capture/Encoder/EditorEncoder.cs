/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using System;

namespace NRKernal.Record
{
    internal class EditorEncoder
    {

        private static EditorEncoder m_Instance;
        public static EditorEncoder GetInstance()
        {
            if (m_Instance == null)
            {
                m_Instance = new EditorEncoder();
            }
            return m_Instance;
        }

        private EditorEncoder()
        {
        }

        public void Register(VideoEncoder encoder)
        {
        }

        public void UnRegister(VideoEncoder encoder)
        {
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void SetConfig(NativeEncodeConfig config)
        {
        }

        public void SetMediaProjection(IntPtr projection)
        {
        }

        public void SetTexture(IntPtr texPtr)
        {
        }

        public void SetAudioData(byte[] data)
        {
        }
    }
}