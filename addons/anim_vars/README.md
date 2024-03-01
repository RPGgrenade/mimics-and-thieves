# AnimVars Plugin
Animation Variables plugin

by RPGgrenade

## Description
A plugin to help facilitate the utility of the `AnimationTreeStateMachine` system which normally uses extremely basic only-true boolean values, with the Advanced Expressions option often unused. This is a plugin that should help make the process a bit easier while also a tad more intuitive to understand for anyone using an `AnimationTree` to handle their animations.

## Installation
First, create an `addons` folder at the root of your project (if one doesn't exist) and drop in the `AnimVars` folder. Once this is done click on `Project Settings` at the top of the screen, then go to the `Plugins` tab, you should now be able to see the plugin as toggleable.

![Screenshot](https://media.discordapp.net/attachments/1132083965700882563/1192959088003395794/image.png?ex=65aaf8ba&is=659883ba&hm=3afd4109b686bbd416f8bffa1c1e613e9df816ff19262f92182912b52ef481ad&=&quality=lossless)

## Usage
Once enabled, you should now have a new Node type appear in the node adding window called `AnimationVariables`.

![Screenshot](https://media.discordapp.net/attachments/1132083965700882563/1192959088208920606/image.png?ex=65aaf8ba&is=659883ba&hm=116e283f14209118ecd7fbfc40c6929c84175e98ae512ea166a5745795ad3d4f&=&quality=lossless)

### Styles
You now have two options on how to use it.

1.- You can create a new Node child on the `AnimationTree` with a script that directly inherits from `AnimVariables.cs`. Then add the properties, variables and functionality you like to be tracked. This still allows utility of the base options but it can get confusing quickly. This is more for people who are familiar with scripting.

2.- You can simply add the `AnimationVariables` node as a child to the `AnimationTree` and use its base utilities as is. It takes a little getting used to but it's fairly simple once you've gotten the hang of it. If you aren't very familiar with scripting, this may be the right method.

### Setting up variables
![Screenshot](https://media.discordapp.net/attachments/798903112798699540/1193893852260614194/image.png?ex=65ae5f4b&is=659bea4b&hm=ae268b60aea66d456707e2885f27ddee65a1ea8bd9582bcba8b5626f2aede051&=&quality=lossless)

The `AnimationVariables` node contains a set of variables of its own for you to set up (if using the second method primarily). The first being the `Anim Tree` variable. This will be set at runtime as the parent if it's an `AnimationTree` so it's not necessary, but if you wanted to set a custom one from a different location it's possible to do so by setting this.

#### Variables List
This is the `Variables` variable you see in its own category. To add to it, simply click on the box on the right side, then click `Add Element` to create an object. Click on the object and select `new AnimVariable` to have a new custom Animation Variable set up! Add as many as you need.

![Screenshot](https://media.discordapp.net/attachments/798903112798699540/1193894877558222869/image.png?ex=65ae6040&is=659beb40&hm=c32281f33adc5f7555fdeef9f1ee87b2ddb92a4298b7c3612f270e8434079e56&=&quality=lossless)

Inside of an Animation Variable you have several options you can set however you like:
- The value (Value) : This will update depenting on what type of variable it is. It keeps track of the current recorded value of said variable for use in other locations.

- The type (Variable Type) : Use this dropdown list to pick what kind of variable it can be. Currently it only accepts `bool`, `float`, `int` and `string`. If other options are needed please let me know. This will automatically change the Value to reflect the type selected.

- The name (Variable Name) : Simply put, the name of the variable. This also acts as the ID of the variable so if getting or setting values, this exact name must be used.

- Is Constant : Determines whether the variable is a constant or not. If set to constant, any attempts at setting the variable won't do anything. Helpful for when you want unvariable values.

#### Scalars List
This is the `Scalars` variable you see in its own category. To add to it, simply click on the box on the right side, then click `Add Element` to create an object. Click on the object and select `new AnimScalarExpression` to have a new custom Animation Scalar Expression set up! Add as many as you need.

![Screenshot](https://media.discordapp.net/attachments/798903112798699540/1193899373931798528/image.png?ex=65ae6470&is=659bef70&hm=5903be25c1df25e506ccd3169fc963bd09b839414f7f1ff276b41441a2652846&=&quality=lossless)

The utility here is more specific than with variables. These are setting "parameters" within the `AnimationTree`. If creating time scales, or other variables that need to be updated in real time, it creates a parameter that can be accessed and given input to change. This is helpful for animations that need to play at different speeds based on other aspects (like walk speed) or other blending options.

Inside of an Animation Scalar Expression you have several options you can set however you like (with some exceptions):
- The name (Name) : This is simply the name of the scalar for the user to keep track with, it has no functional utility.

- The path (Scalar Path) : This is the path to the parameter that needs to be updated. In order to get said path, go to your `AnimationTree` and find the parameter, right click it and select `Copy Property Path`, then paste it here.

- Expression : This is the code that determines how things will work. It can be a number like `20.4f`, however, it's meant to be used in conjunction with prior variables and operations. For example if we have a float variable named `WalkSpeed`, we can apply it here by using `Get("WalkSpeed")`, we can multiply various at once like `(Get("WalkSpeed") * Get("StepHeight")) / 20` and other similar types of expressions can be used. They can be quite complex if needed. However if it is an incorrectly written expression an error will be sent to the output log.

### Using variables
Once all your variables are set up, you can use them in two primary ways.
- `Get(string varName)` : You use this to get variables out of the node class in question. This doesn't change their values (this is usually used inside the animation tree).
- `Set(string varName, Variant value)` : You use this to set variables insiide of the node class in question. This modifies their values if the variables aren't constant (this is usually set through code).

When setting up the transitions of an `AnimationTree`, the way to have it work with these variables is to go into the transition itself and opening the `Advance` group. Inside there'll be a `Mode` (which should be set to `Auto`), a `Condition` (which should be empty unless you wish to use the standard system this plugin replaces), and `Expression`. Inside of `Expression` is where you set the condition you wish to take place in order to have the transition happen. It's important to remember to use the `Get()` function here otherwise it won't work (unless you used the inheritance method)

![Screenshot](https://media.discordapp.net/attachments/798903112798699540/1193903935514230845/image.png?ex=65ae68af&is=659bf3af&hm=76a6737910585d1fb06e8b8cf5bd12ec8e95ace08f3f06b1da3eccf647a70a36&=&quality=lossless)
