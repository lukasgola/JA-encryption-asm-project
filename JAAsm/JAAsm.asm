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
    xor r10, r10                        ; Licznik dla wczytania znaku
    xor rbx, rbx                        ; Licznik dla zapisu znaku
    
TextLoop:
    mov al, [rcx + r10]                 ; Wczytanie bajtu ze Ÿród³a
    ;test al, al                         ; Sprawdzenie, czy bajt to terminator null
    ;jz EndLoop                          ; Jeœli terminator null, zakoñcz pêtlê

                    
    shr al, 2                           ; Pierwsze 6 bitów
    movzx r11, al                       ; Skopiowanie do 64-bitowego rejestru, aby unikn¹æ problemów z rozszerzaniem znaku
    mov al, [r8 + r11]                  ; Pobranie odpowiedniego znaku kodu Base64
    mov [rdx + rbx], al                 ; Zapisanie znaku do bufora wyjœciowego
    inc rbx                             ; Inkrementcja licznika zapisu

                                        ; Stan licznika wczytania 0
                                        ; Stan licznka zapisu 1

    mov al, [rcx + r10]                 ; Ponowne wczytanie pierwszego bajtu
    shl al, 6                           ; Przesuniecie bitowe w lewo o 6 pozycji
    shr al, 2                           ; przesuniecie bitowe w prawo o 6 pozycji
    inc r10                             ; Inkrementacja licznika
    mov [rdx + rbx], al                 ; Tymaczasowe zapianie ostatnich 2 bitow do rejestru wynikowego
    mov al, [rcx + r10]                 ; Wczytanie 2 bajtu
    shr al, 4                           ; Przesuniecie bitowe w prawo o 4 bajty
    add al, [rdx + rbx]                 ; Dodanie tymczasowych 2 bitow w celu uzyskania adresu znku base64
    movzx r11, al                       ; Skopiowanie do 64-bitowego rejestru, aby unikn¹æ problemów z rozszerzaniem znaku
    mov al, [r8 + r11]                  ; Pobranie odpowiedniego znaku kodu Base64
    mov [rdx + rbx], al                 ; Zapisanie znaku bufora wyjsciowego do 2 bajtu
    inc rbx

                                        ; Stan licznika wczytania 1
                                        ; Stan licznika zapisu 2

    mov al, [rcx + r10]                 ; Ponowne wczytanie drugiego bajtu
    shl al, 4                           ; Przesuniecie bitowe w lewo o 4 pozycji
    shr al, 2                           ; Przesuniecie bitowe w prawo o 2 pozycje
    inc r10                             ; Inkrementacja licznika
    mov [rdx + rbx], al                 ; Tymaczasowe zapianie pierwszych 4 bitów bitow do rejestru wynikowego
    mov al, [rcx + r10]                 ; Wczytanie 3 bajtu
    shr al, 6                           ; Przesuniêcie bitowe w prawo o 6 pozycji
    add al, [rdx + rbx]                 ; Dodanie tymczasowych 4 bitow w celu uzyskania adresu znku base64
    movzx r11, al                       ; Skopiowanie do 64-bitowego rejestru, aby unikn¹æ problemów z rozszerzaniem znaku
    mov al, [r8 + r11]                  ; Pobranie odpowiedniego znaku kodu Base64
    mov [rdx + rbx], al                 ; Zapisanie znaku bufora wyjsciowego do 2 bajtu
    inc rbx
                                        ; Stan licznika wczytania 2
                                        ; Stan licznika zapisu 3

    mov al, [rcx + r10]                 ; Ponowne wczytanie trzeciego bajtu
    shl al, 2                           ; Przesuniecie bitowe w lewo o 2
    shr al, 2                           ; Przesuniecie bitowe w prawo o 2
    movzx r11, al                       ; Skopiowanie do 64-bitowego rejestru, aby unikn¹æ problemów z rozszerzaniem znaku
    mov al, [r8 + r11]                  ; Pobranie odpowiedniego znaku kodu Base64
    mov [rdx + rbx], al                 ; Zapisanie znaku bufora wyjsciowego do 3 bajtu

                                        ; Stan licznika wczytania 2
                                        ; Stan licznika zapisu 3

    inc r10                             ; Przejœcie do kolejnego bajtu dla wczytania
    inc rbx                             ; Przejœcie do kolejnego bajtu dla zapisu

    sub r9, 3
    jnz TextLoop                        ; Jeœli licznik nie jest zerowy, kontynuuj pêtlê

EndLoop:
    
    mov rax, r10                        ; Return the destination pointer
    ret
ImageToBase64 endp



end