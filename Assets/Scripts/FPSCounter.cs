using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.Utility
{
    public class FPSCounter : MonoBehaviour
    {
        const float fpsMeasurePeriod = 0.5f;
        private int m_FpsAccumulator = 0;
        private float m_FpsNextPeriod = 0;
		public int m_CurrentFps;
        const string display = "{0}";
		public Text m_GuiText;


        private void Start()
        {
            m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
			m_GuiText = GetComponent<Text>();
        }


        private void Update()
        {
            // measure average frames per second
            m_FpsAccumulator++;
            if (Time.realtimeSinceStartup > m_FpsNextPeriod)
            {
                m_CurrentFps = (int) (m_FpsAccumulator/fpsMeasurePeriod);
                m_FpsAccumulator = 0;
                m_FpsNextPeriod += fpsMeasurePeriod;
                m_GuiText.text = string.Format(display, m_CurrentFps);
				//warn color
				if (m_CurrentFps <= 20) {
					m_GuiText.color = Color.red;
				} else if (m_CurrentFps <= 30) {
					m_GuiText.color = Color.yellow;
				} else {
					m_GuiText.color = Color.white;
				}
            }
        }
    }
}
