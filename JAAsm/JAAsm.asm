.data
    inputData db "Hello, World!", 0      ; Input string to be encoded in Base64
    encodedOutput db 100 dup(0)          ; Buffer to hold the encoded Base64 output

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


; void ImageToBase64(
;	ussigned char* bmpOriginal,		= RCX,
;	int imageSizeInBytes			= RDX,
; )

ImageToBase64 proc
	xor r10, r10

CountLoop:
	inc r10
	dec rdx
	jnz CountLoop

EndLoop:
	mov rax, r10
	ret

ImageToBase64 endp



Base64Encode PROC, inputString:ptr byte
    mov rdx, inputString			; Load address of input string into RDX
    mov rcx, OFFSET encodedOutput	; Load address of output buffer into RCX

encodeLoop:
    movzx rax, byte ptr [rdx]		; Load a byte from the input string
    test rax, rax					; Check for null terminator
    jz endEncodeLoop				; If null terminator is reached, exit the loop

    movzx rbx, al					; Copy the byte to RBX (lower 8 bits)
    shl rax, 16						; Shift the byte 16 bits to the left
    shr rax, 2						; Shift right to get 6 high bits

    ; Extract 6 high bits from AH using bitwise operations with AL (8-bit register)
    mov al, ah						; Move AH to AL
    and al, 0FCh					; Mask to keep only the 6 high bits

    movzx rcx, al					; Move the result to RCX for further processing

    ; Continue with your encoding logic...

    ; Placeholder: Implement the rest of your Base64 encoding logic here

    inc rdx							; Move to the next byte in the input string
    inc rcx							; Move to the next position in the output buffer
    jmp encodeLoop					; Continue encoding the string

endEncodeLoop:
    mov byte ptr [rcx], 0			; Null-terminate the output string
    ret								; Return from the procedure
Base64Encode ENDP


end