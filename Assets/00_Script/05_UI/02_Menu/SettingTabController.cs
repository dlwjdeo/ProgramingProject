using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsTabController : MonoBehaviour
{
    [Serializable]
    public class Tab
    {
        public string id;
        public Toggle toggle;
        public GameObject page;
    }

    [Header("Tabs")]
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private List<Tab> tabs = new List<Tab>();

    [Header("Default")]
    [SerializeField] private int defaultTabIndex = 0;

    private void Awake()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            int index = i;
            var t = tabs[index];

            if (t.toggle == null || t.page == null)
            {
                Debug.LogWarning($"{name}: tabs[{index}]에 toggle/page가 비어있습니다.");
                continue;
            }

            // 토글이 그룹에 속하도록 보장 (인스펙터에서 해도 되지만 안전장치)
            if (toggleGroup != null)
                t.toggle.group = toggleGroup;

            // OnValueChanged는 true/false 둘 다 오니 true일 때만 처리
            t.toggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn) SelectTab(index);
            });
        }
    }

    private void Start()
    {
        // 시작 시 기본 탭 선택
        if (tabs.Count == 0) return;

        defaultTabIndex = Mathf.Clamp(defaultTabIndex, 0, tabs.Count - 1);

        // ToggleGroup이 Allow Switch Off=false면 이 한 줄로 "하나 선택"이 보장됨
        if (tabs[defaultTabIndex].toggle != null)
            tabs[defaultTabIndex].toggle.isOn = true;

        // 혹시 기본 토글이 이미 켜져있어도 페이지 동기화 한 번 보장
        SelectTab(defaultTabIndex);
    }

    public void SelectTab(int index)
    {
        if (index < 0 || index >= tabs.Count) return;

        for (int i = 0; i < tabs.Count; i++)
        {
            var tab = tabs[i];
            if (tab.page == null) continue;

            tab.page.SetActive(i == index);
        }
    }

    public void SelectTab(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        for (int i = 0; i < tabs.Count; i++)
        {
            if (string.Equals(tabs[i].id, id, StringComparison.OrdinalIgnoreCase))
            {
                if (tabs[i].toggle != null)
                    tabs[i].toggle.isOn = true; // 이게 곧 SelectTab(i)로 이어짐
                else
                    SelectTab(i);

                return;
            }
        }
    }
}
