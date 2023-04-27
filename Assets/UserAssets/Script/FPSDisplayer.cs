using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FPSDisplayer : MonoBehaviour {

        private Text text;

        private float frame;
        private float timeElapsed;
        private float frameTime;

        private void Awake() => text = GetComponent<Text>();
        

        private void Update()
        {
            frame++;
            timeElapsed += Time.unscaledDeltaTime;
            if (timeElapsed > 1)
            {
                frameTime = timeElapsed / frame;
                timeElapsed -= 1;
                UpdateText();
                frame = 0;
            }
        }

        private void UpdateText() =>
            text.text = string.Format("FPS : {0}, FrameTime : {1:F2} ms", frame, frameTime * 1000.0f);
    }
}
