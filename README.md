# Moure Orb Shader Unity

This is not my original code, but this open source shader code was almost lost to history. The code is originally by Konstantinos Mourelas released back in 2015 under the zlib license.

I found this while googling for a shader that was similar to the Diablo 3 health / mana UI orb. However, the original site and location for the unity package / shader was no longer available and led to dead URLS.

Luckily, the waybackmachine had his original page, just barely archived, and luckily the provided dropbox url on that page still worked.

I am uploading this to my github, so it can be found in a more public and accessible place. I also made some modifications and a bug fix.

I have included the original unity package from the dropbox link, which also includes the original demo scenes.

By referencing this shader code, you can easily convert it to a Unreal / New Unity Shader Graph.

# Modifications / Bug Fix by metric / arcanistry

1. Updated to use proper floats for full precision of textures and colors etc.
    - This is important for VR headsets especially when it comes to colors / textures.
2. \_LineCol is now properly applied. It was being multiplied in the original source
    - rather than added. The \_LineCol alpha is used as the add power for the \_LineCol.rgb: 0 alpha = nothing; 1 alpha = pure white.
3. Added \_Shape which acts as an overall shape mask.
    - Since, with this material on the UI element, you cannot specify an actual sprite to use as a mask.
    - With this you can specify the overall general shape, rather than just a single sphere, rounded square, or square.
4. Modified the MoureOrb Editor cs file to include the new option to specify the Shape Mask.

# License
Copyright(c) 2015 Konstantinos Mourelas

zlib license

Please see the LICENSE.txt for full details
