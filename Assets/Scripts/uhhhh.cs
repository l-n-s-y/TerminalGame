using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class uhhhh : MonoBehaviour
{
    private int maxLimit = 50;
    [Range(1,50)]
    public int fuckvar;

    public TMP_FontAsset asset;
    public TMP_Text text;

    // Update is called once per frame
    bool doInc = false;
    void Update()
    {
        // asset.samplingPointSize = value;
        /*Debug.Log(asset.creationSettings.pointSize);
        Debug.Log(asset.creationSettings.pointSizeSamplingMode);
        asset.creationSettings.pointSizeSamplingMode = fuckvar;*/

        if (doInc) {
            fuckvar += 1;
            if (fuckvar >= maxLimit) {
                doInc = false;
                fuckvar = maxLimit;
            }
        } else {
            fuckvar -= 1;
            if (fuckvar <= 1) {
                doInc = true;
                fuckvar = 1;
            }
        }

        text.font = TMP_FontAsset.CreateFontAsset(
            asset.sourceFontFile,
            fuckvar,
            asset.atlasPadding,
            asset.atlasRenderMode,
            asset.atlasWidth,
            asset.atlasHeight);

        //Font font, int samplingPointSize, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight

    }
}
