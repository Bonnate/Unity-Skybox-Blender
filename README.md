## Skybox Blender
Blend the skybox in unity to change the sky during runtime.
You can set Directional Light, Fog, and Skybox Reflection Intensity.

![thumb gif](https://blog.kakaocdn.net/dn/rxUVJ/btr2YakPZ1N/bjkqGpR5xdJbBxQq1TZvW0/img.gif)


## How to use?
 1. Set the material that uses the VertBlendedSkybox shader to the SkyBox material.
![step 1](https://img1.daumcdn.net/thumb/R1280x0/?scode=mtistory2&fname=https%3A%2F%2Fblog.kakaocdn.net%2Fdn%2FcorEh7%2Fbtr2RYedduH%2FRbyUJrHL2uTQIbrpiE5ym0%2Fimg.png)
 - This shaders and material files are uploaded to the GitHub project.
<br><br>
 2. Create a scriptable object to store the preset.
![step 2](https://img1.daumcdn.net/thumb/R1280x0/?scode=mtistory2&fname=https%3A%2F%2Fblog.kakaocdn.net%2Fdn%2FyHVdd%2Fbtr2GKoleBh%2FD1MyLiQa0gYaafQ3W5oQ4k%2Fimg.png)

 - In the project, click Create > Preset < Environment Preset to create a new scriptable object.

<br><br>

3. Insert the Skybox Preset of the scriptable object file as appropriate.
![step 3](https://img1.daumcdn.net/thumb/R1280x0/?scode=mtistory2&fname=https%3A%2F%2Fblog.kakaocdn.net%2Fdn%2Fc4QUcD%2Fbtr2OQnEnNc%2FDfF0IrkIZ6MYFjqCZxAjx0%2Fimg.png)
 - Insert 6 image files according to the 6 side.

<br><br>

 4. Create an object that uses EnviromentManager.cs in the scene.
![enter image description here](https://img1.daumcdn.net/thumb/R1280x0/?scode=mtistory2&fname=https://blog.kakaocdn.net/dn/nSXV6/btr2RW1Joep/0TsUxPnG2L6k997Q1KrkG1/img.png)
 - Insert the generated presets into Presets.
 - You also set Directional Light and RotSpeed.
<br><br>
5. Call the script to complete Sky Blend using the preset you set.

<br>

    public  class  SampleScript : MonoBehaviour 
    { 
	    private  void  Start() 
	    { 
		    StartCoroutine(CoBlendSkies()); 
	    } 
	    
	    private IEnumerator CoBlendSkies() 
	    { 
		    while (true) 
		    { 
			    EnviromentManager.Instance.BlendEnviroment("Mid", 5.0f); 
			    yield  return  new  WaitForSeconds(10.0f); 
			    EnviromentManager.Instance.BlendEnviroment("Night", 5.0f); 
			    yield  return  new  WaitForSeconds(10.0f);
			    EnviromentManager.Instance.BlendEnviroment("Day", 5.0f);
			    yield  return  new  WaitForSeconds(10.0f);  
		    } 
	    } 
    }

<br>

 - You can call with the name of the set file as a key in a single tone, such as EnvironmentManager.Instance.BlendEnvironment("Mid", 5.0f).
 <br><br><br>
## References
The project uploaded on GitHub was uploaded using free Asset.
https://assetstore.unity.com/packages/3d/environments/lowpoly-environment-nature-pack-free-187052

https://assetstore.unity.com/packages/2d/textures-materials/sky/skybox-series-free-103633

The shader used in the project referred to the file here.
https://gist.github.com/tolotratlt/0cf71ff58a7e7c37a235ec38c6e8b99e
