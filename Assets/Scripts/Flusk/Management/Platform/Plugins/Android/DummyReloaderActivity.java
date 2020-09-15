package com.tfbgames;

import android.app.Activity;
import android.os.Bundle;
import android.content.Intent;
import android.util.Log;

public class DummyReloaderActivity extends Activity
{
	protected void onCreate(Bundle savedInstanceState)
	{
	    Log.d("ReloaderActivity", "Got here");

		super.onCreate(savedInstanceState);
		finish();
	}

	public static void Launch(Activity activity)
	{
		Intent myIntent = new Intent(activity, DummyReloaderActivity.class);
		activity.startActivity(myIntent);
	}
}
