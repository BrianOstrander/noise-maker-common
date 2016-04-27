Summary:

You can move this folder or the dll files in it anywhere in your unity project structure but they must stay together in a single folder.

*** IGNORE THE REST OF THIS INFORMATION if you don't implement any unity component which inherits the components imported from these dlls.

If your components inherit from any component in these dlls, and if your components are declared in a prebuilt dll project, then you must place your dll into the same folder along with those original dlls.

This rule doesn't apply to you if your components are declared as open script files in your unity project. If you don't know what a prebuilt dll project is, you can totally ignore this note. That means you are developing your project as open script files which is the regular unity development method.


Reason:

Unity has a bug that it doesn't recognize or import the components which has ALL of the conditions below...

A) The component is implemented in a prebuilt dll. (Not in a open script file)
B) The component inherits another component which also is implemented in a prebuilt dll (Separate dll file).
C) Those separate dll files reside in separate folders.

Solution:

Only workaround for this bug is to keep all dll files in the same folder so, Unity can import and recognize all the components which are declared in those dlls.


More info:

Link 1) http://answers.unity3d.com/questions/240985/subclassed-monobehavior-in-external-dlls-not-recog.html

Link 2) http://answers.unity3d.com/questions/594712/monobehaviours-editors-with-inheritance-across-dll.html