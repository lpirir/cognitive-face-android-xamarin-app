using System;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace com.rcervantes.xamarinfaceapi_droid.helpers
{
    public class EmbeddedGridView : GridView
    {
        public EmbeddedGridView(Context context, Android.Util.IAttributeSet attributes) 
            : base(context, attributes) { }
        
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
        	base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        	int newHeightMeasureSpec = MeasureSpec.MakeMeasureSpec(Android.Views.View.MeasuredSizeMask, MeasureSpecMode.AtMost);

            //LayoutParameters.Height = this.SelectedView.MeasuredHeight;
        }

    }
}
