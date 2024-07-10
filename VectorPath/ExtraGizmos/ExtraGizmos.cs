using UnityEngine;

namespace VectorPath {

    public static class ExtraGizmos
    {
        /// <summary>
        /// Draws a 2D gizmo arrow from the position in the direction you provide.
        /// It can further be modified by a color, length, the length and angle of the arrow head.
        /// </summary>
        /// <param name="position"> Origion position for the arrow. </param>
        /// <param name="direction"> Direction of arrow. </param>
        /// <param name="color"></param>
        /// <param name="length"></param>
        /// <param name="arrowHeadLength"></param>
        /// <param name="arrowHeadAngle"></param>
        public static void DrawArrow2D(Vector2 position, Vector2 direction, Color color, float length, float arrowHeadLength = 0.25f, float arrowHeadAngle = 10.0f) {
            Gizmos.color = color;

            //Calculating arrowhead position
            direction.Normalize();
            Vector2 arrowHeadPosition = position + direction * length;

            //Calculating arrowhead directions
            arrowHeadAngle -= 180f;
            arrowHeadAngle = arrowHeadAngle * Mathf.Deg2Rad;
            Vector2 right = new Vector2(direction.x * Mathf.Cos(arrowHeadAngle) - direction.y * Mathf.Sin(arrowHeadAngle),
                                        direction.x * Mathf.Sin(arrowHeadAngle) + direction.y * Mathf.Cos(arrowHeadAngle));
            Vector2 left = new Vector2(direction.x * Mathf.Cos(-arrowHeadAngle) - direction.y * Mathf.Sin(-arrowHeadAngle),
                                        direction.x * Mathf.Sin(-arrowHeadAngle) + direction.y * Mathf.Cos(-arrowHeadAngle));
            right.Normalize();
            left.Normalize();

            //Draw Gizmos
            Gizmos.DrawLine(position, arrowHeadPosition);
            Gizmos.DrawLine(arrowHeadPosition, arrowHeadPosition + left * arrowHeadLength);
            Gizmos.DrawLine(arrowHeadPosition, arrowHeadPosition + right * arrowHeadLength);
        }

        /// <summary>
        /// Draws a 2D arrow centered at a given position with a specified direction, color, and length.
        /// </summary>
        /// <param name="position">The center position of the arrow.</param>
        /// <param name="direction">The direction in which the arrow points.</param>
        /// <param name="color">The color of the arrow.</param>
        /// <param name="length">The total length of the arrow.</param>
        /// <param name="arrowHeadLength">The length of the arrowhead. Default is 0.25f.</param>
        /// <param name="arrowHeadAngle">The angle of the arrowhead. Default is 10.0f.</param>
        public static void DrawArrow2DCentered(Vector2 position, Vector2 direction, Color color, float length, float arrowHeadLength = 0.25f, float arrowHeadAngle = 10.0f) {
            Vector2 newPos = new Vector2(position.x - (0.5f * direction.x * length), position.y - (0.5f * direction.y * length));
            DrawArrow2D(newPos, direction, color, length, arrowHeadLength, arrowHeadAngle);
        }

        /// <summary>
        /// Draws a centered X shape in 2D using Gizmos with a specified position, color, and size.
        /// </summary>
        /// <param name="position">The center position of the X shape.</param>
        /// <param name="color">The color of the X shape.</param>
        /// <param name="size">The size of the X shape.</param>
        public static void DrawX2DCentered(Vector2 position, Color color, float size) {
            Gizmos.color = color;
            Gizmos.DrawLine(position + new Vector2(-0.7f, 0.7f) * 0.5f * size, position + new Vector2(0.7f, -0.7f) * 0.5f * size);
            Gizmos.DrawLine(position + new Vector2(-0.7f, -0.7f) * 0.5f * size, position + new Vector2(0.7f, 0.7f) * 0.5f * size);
        }

        /// <summary>
        /// Draws a digit as a Gizmo.
        /// </summary>
        /// <param name="pos"> Position of digit. </param>
        /// <param name="height"> Height of digit. </param>
        /// <param name="digit"> Digit to show. </param>
        public static void DrawDigit(Vector3 pos, float height, int digit) {
            Vector3[] numberPositions = new Vector3[6];
            numberPositions[0] = new Vector3(pos.x - 0.25f * height, pos.y + 0.5f * height);
            numberPositions[1] = new Vector3(pos.x + 0.25f * height, pos.y + 0.5f * height);
            numberPositions[2] = new Vector3(pos.x - 0.25f * height, pos.y);
            numberPositions[3] = new Vector3(pos.x + 0.25f * height, pos.y);
            numberPositions[4] = new Vector3(pos.x - 0.25f * height, pos.y - 0.5f * height);
            numberPositions[5] = new Vector3(pos.x + 0.25f * height, pos.y - 0.5f * height);
            
            switch(digit) {
                case 0:
                    Gizmos.DrawLine(numberPositions[0], numberPositions[1]);
                    Gizmos.DrawLine(numberPositions[1], numberPositions[3]);
                    Gizmos.DrawLine(numberPositions[0], numberPositions[2]);
                    Gizmos.DrawLine(numberPositions[2], numberPositions[4]);
                    Gizmos.DrawLine(numberPositions[3], numberPositions[5]);
                    Gizmos.DrawLine(numberPositions[4], numberPositions[5]);
                    break;
                case 1:
                    Gizmos.DrawLine(numberPositions[1], numberPositions[3]);
                    Gizmos.DrawLine(numberPositions[3], numberPositions[5]);
                    break;
                case 2:
                    Gizmos.DrawLine(numberPositions[0], numberPositions[1]);
                    Gizmos.DrawLine(numberPositions[1], numberPositions[3]);
                    Gizmos.DrawLine(numberPositions[2], numberPositions[3]);
                    Gizmos.DrawLine(numberPositions[2], numberPositions[4]);
                    Gizmos.DrawLine(numberPositions[4], numberPositions[5]);
                    break;
                case 3:
                    Gizmos.DrawLine(numberPositions[0], numberPositions[1]);
                    Gizmos.DrawLine(numberPositions[1], numberPositions[3]);
                    Gizmos.DrawLine(numberPositions[2], numberPositions[3]);
                    Gizmos.DrawLine(numberPositions[3], numberPositions[5]);
                    Gizmos.DrawLine(numberPositions[4], numberPositions[5]);
                    break;
                case 4:
                    Gizmos.DrawLine(numberPositions[1], numberPositions[3]);
                    Gizmos.DrawLine(numberPositions[0], numberPositions[2]);
                    Gizmos.DrawLine(numberPositions[2], numberPositions[3]);
                    Gizmos.DrawLine(numberPositions[3], numberPositions[5]);
                    break;
                case 5:
                    Gizmos.DrawLine(numberPositions[0], numberPositions[1]);
                    Gizmos.DrawLine(numberPositions[0], numberPositions[2]);
                    Gizmos.DrawLine(numberPositions[2], numberPositions[3]);
                    Gizmos.DrawLine(numberPositions[3], numberPositions[5]);
                    Gizmos.DrawLine(numberPositions[4], numberPositions[5]);
                    break;
                case 6:
                    Gizmos.DrawLine(numberPositions[0], numberPositions[1]);
                    Gizmos.DrawLine(numberPositions[0], numberPositions[2]);
                    Gizmos.DrawLine(numberPositions[2], numberPositions[3]);
                    Gizmos.DrawLine(numberPositions[2], numberPositions[4]);
                    Gizmos.DrawLine(numberPositions[3], numberPositions[5]);
                    Gizmos.DrawLine(numberPositions[4], numberPositions[5]);
                    break;
                case 7:
                    Gizmos.DrawLine(numberPositions[0], numberPositions[1]);
                    Gizmos.DrawLine(numberPositions[1], numberPositions[3]);
                    Gizmos.DrawLine(numberPositions[3], numberPositions[5]);
                    break;
                case 8:
                    Gizmos.DrawLine(numberPositions[0], numberPositions[1]);
                    Gizmos.DrawLine(numberPositions[1], numberPositions[3]);
                    Gizmos.DrawLine(numberPositions[0], numberPositions[2]);
                    Gizmos.DrawLine(numberPositions[2], numberPositions[3]);
                    Gizmos.DrawLine(numberPositions[2], numberPositions[4]);
                    Gizmos.DrawLine(numberPositions[3], numberPositions[5]);
                    Gizmos.DrawLine(numberPositions[4], numberPositions[5]);
                    break;
                case 9:
                    Gizmos.DrawLine(numberPositions[0], numberPositions[1]);
                    Gizmos.DrawLine(numberPositions[1], numberPositions[3]);
                    Gizmos.DrawLine(numberPositions[0], numberPositions[2]);
                    Gizmos.DrawLine(numberPositions[2], numberPositions[3]);
                    Gizmos.DrawLine(numberPositions[3], numberPositions[5]);
                    Gizmos.DrawLine(numberPositions[4], numberPositions[5]);
                    break;
                default:
                    break;
            }  
        }

        /// <summary>
        /// Draws a sequence of digits as Gizmos at a specified position in 3D space, with each digit having a specified height and padding.
        /// </summary>
        /// <param name="pos">The position where the number should be drawn.</param>
        /// <param name="height">The height of each digit.</param>
        /// <param name="number">The integer number to be drawn.</param>
        /// <param name="color">The color of the digits.</param>
        /// <param name="padding">Optional. The padding between digits. Default is 0.1f.</param>
        public static void DrawNumber(Vector3 pos, float height, int number, Color color, float padding = 0.1f) {
            Gizmos.color = color;
            int[] digits = GetDigits(number);
            float offset;
            if(digits.Length % 2 == 0) {
                offset = ((digits.Length / 2) - 1) * 0.5f * height + ((digits.Length / 2) - 1) * padding + 0.5f * padding + 0.25f * height;
            }
            else {
                offset = (digits.Length - 1) / 2 * 0.5f * height + digits.Length / 2 * padding;
            }
            for (int i = 0; i < digits.Length; i++) {
                DrawDigit(new Vector3(pos.x - offset + (i * 0.5f * height + i * padding), pos.y, pos.z), height, digits[i]);
            }
        }

        private static int[] GetDigits(int number) {
            if(number == 0) {
                return new int[]{0};
            }
            int[] result = new int[Mathf.FloorToInt(Mathf.Log10(number) + 1)];
            for (int i = 0; i < result.Length; i++) {
                result[result.Length - 1 - i] = number % 10;
                number /= 10;
            }
            return result;
        }
    }
}
