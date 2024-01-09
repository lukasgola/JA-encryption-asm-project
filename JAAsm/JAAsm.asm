.data
    inputData db "Hello, World!", 0      ; Input string to be encoded in Base64
    encodedOutput db 100 dup(0)          ; Buffer to hold the encoded Base64 output
    

.code
; void ASMAdjustBrightness(
;	unsigned char* bmpDataScan0,	= RCX,
;	unsigned char* bmpOriginal,		= RDX,
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





; int TextToBase64(
;	unsigned char* inputString,		= RCX,
;   unsigned char* outputString     = RDX,
;   unsigned char* base64_table     = R8,
;	int imageSizeInBytes			= R9,
; )

ImageToBase64 proc
    xor r10, r10
    
TextLoop:
    mov al, [rcx + r10]       ; Wczytanie bajtu ze Ÿród³a
    test al, al             ; Sprawdzenie, czy bajt to terminator null
    jz EndLoop              ; Jeœli terminator null, zakoñcz pêtlê

    ; Operacje na kodzie Base64 dla pojedynczych bajtów
    shr al, 2               ; Pierwsze 6 bitów
    movzx r11, al           ; Skopiowanie do 64-bitowego rejestru, aby unikn¹æ problemów z rozszerzaniem znaku
    mov al, [r8 + r11]      ; Pobranie odpowiedniego znaku kodu Base64
    mov [rdx + r10], al       ; Zapisanie znaku do bufora wyjœciowego


    mov al, [rcx + r10]
    shl al, 6
    shr al, 2
    inc r10
    mov [rdx + r10], al
    mov al, [rcx+r10]    ; Skopiowanie do 64-bitowego rejestru, aby unikn¹æ problemów z rozszerzaniem znaku
    shr al, 4
    add al, [rdx+r10]
    movzx r11, al
    mov al, [r8 + r11]
    mov [rdx + r10], al


    mov al, [rcx + r10]
    shl al, 4
    shr al, 2
    inc r10
    mov [rdx + r10], al
    mov al, [rcx + r10]
    shr al, 6
    add al, [rdx + r10]
    movzx r11, al
    mov al, [r8 + r11]
    mov [rdx + r10], al


    mov al, [rcx + r10]
    shl al, 2
    shr al, 2
    inc r10
    movzx r11, al
    mov al, [r8 + r11]
    mov [rdx + r10], al




    inc r10                 ; Przejœcie do kolejnego bajtu
    inc r10                 ; Pomiñ dwa znaki w buforze wyjœciowym

    dec r9                  ; Dekrementacja licznika
    jnz TextLoop            ; Jeœli licznik nie jest zerowy, kontynuuj pêtlê

EndLoop:
    mov rax, rdx        ; Return the destination pointer
    ret
ImageToBase64 endp



end