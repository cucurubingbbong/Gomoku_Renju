# Omok Gameplay Light Models

실제 오목 대국 화면용 경량 모델 팩입니다. 바둑판은 포함하지 않습니다.

## 핵심
- CherryTree LOD0 / LOD1 / LOD2
- JapanesePavilion LOD0 / LOD1 / LOD2
- 경량 돗자리, 돌통, 뚜껑, 흑돌, 백돌
- 흑돌/백돌 40개 세트: 각각 별도 Transform, 하나의 공유 메시

## Unity 권장 LOD
- LOD0: 화면 점유율 45% 이상
- LOD1: 20% 이상
- LOD2: 7% 이상
- 7% 미만: Cull 또는 Billboard

꽃과 잔디의 실시간 그림자는 끄거나 Near Only로 제한하세요.
