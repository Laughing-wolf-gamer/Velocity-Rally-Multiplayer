using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(ScrollRect),typeof(RectTransform))]
public class ScrollSelect : MonoBehaviour {
    public float scrollSpeed = 10f;

    private ScrollRect m_ScrollRect;
    private RectTransform m_RectTransform;
    private RectTransform m_ContentRectTransform;
    private RectTransform m_SelectedRectTransform;
    private GameObject selected;
    private void Awake() {
        m_ScrollRect = GetComponent<ScrollRect>();
        m_RectTransform = GetComponent<RectTransform>();
        m_ContentRectTransform = m_ScrollRect.content;
    }

    private void Update() {
        UpdateScrollToSelected();
    }

    private void UpdateScrollToSelected() {
        // grab the current selected from the eventsystem
        // GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return;
        
        if (selected.transform.parent != m_ContentRectTransform.transform) return;
        
        if(selected.TryGetComponent(out RectTransform curretnRect)){
            m_SelectedRectTransform = curretnRect;
        }
        // math stuff
        Vector3 selectedDifference = m_RectTransform.localPosition - m_SelectedRectTransform.localPosition;
        float contentHeightDifference = (m_ContentRectTransform.rect.height - m_RectTransform.rect.height);

        float selectedPosition = (m_ContentRectTransform.rect.height - selectedDifference.y);
        float currentScrollRectPosition = m_ScrollRect.normalizedPosition.y * contentHeightDifference;
        float above = currentScrollRectPosition - (m_SelectedRectTransform.rect.height / 2) + m_RectTransform.rect.height;
        float below = currentScrollRectPosition + (m_SelectedRectTransform.rect.height / 2);

        // check if selected is out of bounds
        if (selectedPosition > above) {
            float step = selectedPosition - above;
            float newY = currentScrollRectPosition + step;
            float newNormalizedY = newY / contentHeightDifference;
            m_ScrollRect.normalizedPosition = Vector2.Lerp(m_ScrollRect.normalizedPosition, new Vector2(0, newNormalizedY), scrollSpeed * Time.deltaTime);
        } else if (selectedPosition < below) {
            float step = selectedPosition - below;
            float newY = currentScrollRectPosition + step;
            float newNormalizedY = newY / contentHeightDifference;
            m_ScrollRect.normalizedPosition = Vector2.Lerp(m_ScrollRect.normalizedPosition, new Vector2(0, newNormalizedY), scrollSpeed * Time.deltaTime);
        }else{
            selected = null;
        }
    }
    public void SetSelected(GameObject selected){
        this.selected = selected;
    }
}