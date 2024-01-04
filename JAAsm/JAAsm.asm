.data

.code
; void ASMAdjustBrightness(
;	unsigned char* bmpDataScan0,	= RCX,
;	ussigned char* bmpOriginal,		= RDX,
;	short value,					= R8W,
;	int imageSizeInBytes			= R9D,
; )
ASMAdjustBrightness proc

	mov r10, 0						; Set the offset pointer to 0
	cmp r8w, 0						; Check if we - or +
	jl SubstractBrightness

	mov r11w, 0ffffh				; Set my overflow value to 255

MainLoopAdd:
	mov al, byte ptr [rdx+r10]		; Read the next byte from orig
	add al, r8b						; Add the brightness
	cmovc ax, r11w					; Set to 255 on overflow
	mov byte ptr [rcx + r10], al	

	inc r10							; Move to the next byte in the array
	dec r9d							; Decrement our counter
	jnz MainLoopAdd					; Jump if there's more
	ret								; return to C#

SubstractBrightness:
	mov r11w, 0						; Move 0 into the underflow
	neg r8w							; Negate the value as we're subbing

MainLoopSub:
	mov al, byte ptr [rdx+r10]		; Read the next byte from orig
	sub al, r8b						; Sub the brightness
	cmovc ax, r11w					; Set to 255 on overflow
	mov byte ptr [rcx + r10], al	

	inc r10							; Move to the next byte in the array
	dec r9d							; Decrement our counter
	jnz MainLoopSub					; Jump if there's more
	ret	

ASMAdjustBrightness endp
end