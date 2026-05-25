using System;
using UnityEngine;
using UnityEngine.UI;

public class UpdateResourceForm : MonoBehaviour
{
    [System.Serializable]
    public struct UpdateData
    {
        public Text m_TipText;
        public Text m_UpdateText;
        public Text m_LeftTimeText;
        public Slider m_ProgressSlider;
    }
    [System.Serializable]
    public struct UnzipData
    {
        public Text m_UnzipText;
        public Slider m_ProgressSlider;
    }

    [SerializeField]
    private GameObject m_UpdateGo = null;
    [SerializeField]
    private UpdateData m_UpdateData;
    [SerializeField]
    private GameObject m_UnzipGo = null;
    [SerializeField]
    private UnzipData m_UnzipData;

    public static UpdateResourceForm Inst { get; private set; } = null;

    private void Awake()
    {
        //m_UnzipGo.gameObject.SetActive(false);
        m_UpdateGo.gameObject.SetActive(false);
        Inst = this;
    }

    public void SetUpdateProgress(float progress, string description, float leftTime)
    {
        //m_UnzipGo.gameObject.SetActive(false);
        m_UpdateGo.gameObject.SetActive(true);
        m_UpdateData.m_TipText.text = "更新过程中请不要关闭手机";

        m_UpdateData.m_ProgressSlider.value = progress;
        m_UpdateData.m_UpdateText.text = description;

        m_UpdateData.m_LeftTimeText.text = $"预计剩余时间: {((int)leftTime / 60) + 1}分钟";
    }

    public void SetPreloadProgress(float progress)
    {
        m_UnzipGo.gameObject.SetActive(true);
        m_UpdateGo.gameObject.SetActive(false);
        m_UnzipData.m_ProgressSlider.value = progress;
        m_UnzipData.m_UnzipText.text = "正在解压资源...";
    }
}