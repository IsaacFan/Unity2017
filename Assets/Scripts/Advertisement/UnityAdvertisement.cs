using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;


public class UnityAdvertisement : MonoBehaviour {

    public Button watchAdvertisementButton;
    public Text advertisementResultTest;


	
	void Awake () {

        if (Advertisement.isSupported == false)
            watchAdvertisementButton.enabled = false;
        else
            watchAdvertisementButton.onClick.AddListener(onClickWatchAdvertisementButton);

    }
	
	private void onClickWatchAdvertisementButton()
    {
        if (Advertisement.IsReady() == false)
        {
            advertisementResultTest.text = "Advertisement is not ready !";
            return;
        }

        ShowOptions options = new ShowOptions();
        options.resultCallback = handleShowResult;

        Advertisement.Show("rewardedVideo", options);
    }
    private void handleShowResult(ShowResult showResult)
    {
        switch (showResult)
        {
            default:
            case ShowResult.Failed:
                advertisementResultTest.text = "Advertisement failed to show !";
                break;
            case ShowResult.Skipped:
                advertisementResultTest.text = "Advertisement was skipped !";
                break;
            case ShowResult.Finished:
                advertisementResultTest.text = "Advertisement completed !";
                giveReward();
                break;
        }
    }

    private void giveReward()
    {

    }

}
