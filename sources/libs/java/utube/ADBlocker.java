package utube;

import android.view.View;
import android.view.ViewGroup;
import android.widget.FrameLayout;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.Toolbar;

import java.nio.ByteBuffer;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.List;

public class ADBlocker
{
    public static boolean LithoView(String value, ByteBuffer buffer)
    {
        List<String> templateBlockList = new ArrayList<>();
        List<String> bufferBlockList = new ArrayList<>();

        bufferBlockList.add("ad_cpn");
        bufferBlockList.add("watch-vrecH");
        bufferBlockList.add("YouTube Movies");

        templateBlockList.add("_ad");
        templateBlockList.add("ad_badge");
        templateBlockList.add("ads_video_with_context");
        templateBlockList.add("cell_divider");
        templateBlockList.add("comments_composite_entry_point");
        templateBlockList.add("comments_entry_point_message");
        templateBlockList.add("community_guidelines");
        templateBlockList.add("compact_banner");
        templateBlockList.add("compact_movie");
        templateBlockList.add("compact_tvfilm_item");
        templateBlockList.add("emergency_onebox");
        templateBlockList.add("horizontal_movie_shelf");
        templateBlockList.add("horizontal_video_shelf");
        templateBlockList.add("in_feed_survey");
        templateBlockList.add("medical_panel");
        templateBlockList.add("movie_and_show_upsell_card");
        templateBlockList.add("paid_content_overlay");
        templateBlockList.add("post_base_wrapper");
        templateBlockList.add("post_shelf");
        templateBlockList.add("product_carousel");
        templateBlockList.add("publisher_transparency_panel");
        templateBlockList.add("reels_player_overlay");
        templateBlockList.add("shelf_header");
        templateBlockList.add("shorts_shelf");
        templateBlockList.add("single_item_information_panel");
        templateBlockList.add("watch_metadata_app_promo");

        return ((containsAny(value,
                    "home_video_with_context",
                    "related_video_with_context") &&
                bufferBlockList.stream().anyMatch(StandardCharsets.UTF_8.decode(buffer).toString()::contains))
                ||
                (!containsAny(value,
                        "home_video_with_context",
                        "related_video_with_context",
                        "search_video_with_context",
                        "menu",
                        "root",
                        "-count",
                        "-space",
                        "-button") &&
                        templateBlockList.stream().anyMatch(value::contains)));
    }
    private static boolean containsAny (String value, String... targets)
    {
        for (String string : targets)
        {
            if (value.contains((string)))
            {
                return true;
            }
        }
        return false;
    }

    public static void LayoutView(View view) {
        if (view instanceof LinearLayout) {
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(1, 1);
            view.setLayoutParams(layoutParams);
        } else if (view instanceof FrameLayout) {
            FrameLayout.LayoutParams layoutParams2 = new FrameLayout.LayoutParams(1, 1);
            view.setLayoutParams(layoutParams2);
        } else if (view instanceof RelativeLayout) {
            RelativeLayout.LayoutParams layoutParams3 = new RelativeLayout.LayoutParams(1, 1);
            view.setLayoutParams(layoutParams3);
        } else if (view instanceof Toolbar) {
            Toolbar.LayoutParams layoutParams4 = new Toolbar.LayoutParams(1, 1);
            view.setLayoutParams(layoutParams4);
        } else if (view instanceof ViewGroup) {
            ViewGroup.LayoutParams layoutParams5 = new ViewGroup.LayoutParams(1, 1);
            view.setLayoutParams(layoutParams5);
        }
    }

    public static void NormalView(View view) {
        view.setVisibility(View.GONE);
    }

    public static Enum lastPivotTab;
    public static void HideShortsButton(View view) {
        if (lastPivotTab != null && lastPivotTab.name() == "TAB_SHORTS") {
            view.setVisibility(View.GONE);
        }
    }
}
