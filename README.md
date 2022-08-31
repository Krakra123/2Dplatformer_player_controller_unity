This is a 2D Platformer Player controller script support basic movement and jumping
Require standard Unity Physic2D and Unity InputSystem (now only support BoxCollider2D, I'll update in the future)

Component explain:
* Input:
  - Movement Input Axis: Input axis name of horizontal movement (you can see in Input manager) (Unity default: "Horizontal")
  - Jump Input Button: Input button name of jump button (Unity default: "Jump")
* Ground check: Check if the character touch the ground (the main reason why this script only support BoxCollider2D)
  - Ground Layer Mask: Layer of the ground
* Running: Horizontal movement of the character
  - Running Max Speed: Max velocity the character can get
  - Running Speed Acceleration: How fast the character velocity can gets to the "Running Max Speed" while on the ground if you keep running in the direction of the character facing
  - Running Speed Deceleration: How fast the character velocity gets down to "0" while on the ground if you stop while running
  - Running Speed Turn: How fast the character can change the running direction while on the ground
* Jumping: Jump
  - Jump Height: Max jump force of the character
  - Gravity Scale: Scale of gravity (gravity = gravity * Gravity Scale)
  - On Air Down Gravity Scale: Scale of gravity while the character fall down (gravity = gravity * Gravity Scale * On Air Down Gravity Scale)
  - On Air Speed Acceleration: How fast the character velocity can gets to the "Running Max Speed" while on the air if you keep running in the direction of the character facing
  - On Air Speed Control: How fast the character can change the direction/velocity while on air
  - Variable Jump Cut Off: Hold down the jump button to jump higher, the bigger the number, the more effect you get ("0" mean no Variable Jump)
* Other: some assistant
  - Coyote Time: Time (seconds) the character can still jump if not on the ground
  - Jump Buffer: Time (seconds) the character can still jump as soon as the character touch the ground if you jump too soon when the charaction not on the ground
  - Terminal Velocity: Min Y velocity 
  
