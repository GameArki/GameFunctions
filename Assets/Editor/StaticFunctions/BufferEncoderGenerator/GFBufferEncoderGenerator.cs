using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using GameFunctions.CSharpGenerator;

namespace GameFunctions.Editors {

    public class GFBufferEncoderGenerator {

        const string n_WriteTo = "WriteTo";
        const string n_FromBytes = "FromBytes";

        static string ATTR = nameof(GFBufferEncoderMessageAttribute).Replace("Attribute", "");

        public static void GenModel(string inputDir) {

            List<string> files = FindAllFileWithExt(inputDir, "*.cs");
            files.ForEach(value => {
                string code = File.ReadAllText(value);
                GFClassEditor classEditor = new GFClassEditor();
                classEditor.LoadCode(code);
                bool hasAttr = classEditor.HasClassAttribute(ATTR);
                if (!hasAttr) {
                    return;
                }

                MethodEditor writeToMethod = GenWriteToMethod(inputDir, classEditor);
                classEditor.RemoveMethod(writeToMethod.GetName());
                classEditor.AddMethod(writeToMethod);

                MethodEditor fromBytesMethod = GenFromBytesMethod(inputDir, classEditor);
                classEditor.RemoveMethod(fromBytesMethod.GetName());
                classEditor.AddMethod(fromBytesMethod);

                string typeName = $"{typeof(IGFBufferEncoderMessage<>).Name.Replace("`1", "")}<{classEditor.GetClassName()}>";
                classEditor.RemoveInherit(typeName);
                classEditor.InheritInterface(typeName);

                classEditor.AddUsing(nameof(System));
                classEditor.AddUsing(nameof(GameFunctions));

                File.WriteAllText(value, classEditor.Generate());

            });

        }

        static MethodEditor GenWriteToMethod(string inputDir, GFClassEditor classEditor) {

            const string t_dst = "byte[]";
            const string n_dst = "dst";
            const string t_ref_int = "ref int";
            const string n_offset = "offset";

            string WriteLine(string t_field, string n_field) {
                string paramSuffix = $"{n_dst}, {n_field}, ref {n_offset}";
                switch (t_field) {
                    case "byte": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteUInt8)}({paramSuffix});";
                    case "byte[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteUint8Arr)}({paramSuffix});";
                    case "List<byte>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteUint8List)}({paramSuffix});";
                    case "sbyte": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteInt8)}({paramSuffix});";
                    case "sbyte[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteInt8Arr)}({paramSuffix});";
                    case "List<sbyte>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteInt8List)}({paramSuffix});";
                    case "bool": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteBool)}({paramSuffix});";
                    case "bool[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteBoolArr)}({paramSuffix});";
                    case "List<bool>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteBoolList)}({paramSuffix});";
                    case "short": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteInt16)}({paramSuffix});";
                    case "short[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteInt16Arr)}({paramSuffix});";
                    case "List<short>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteInt16List)}({paramSuffix});";
                    case "ushort": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteUInt16)}({paramSuffix});";
                    case "ushort[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteUInt16Arr)}({paramSuffix});";
                    case "List<ushort>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteUInt16List)}({paramSuffix});";
                    case "int": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteInt32)}({paramSuffix});";
                    case "int[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteInt32Arr)}({paramSuffix});";
                    case "List<int>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteInt32List)}({paramSuffix});";
                    case "uint": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteUInt32)}({paramSuffix});";
                    case "uint[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteUInt32Arr)}({paramSuffix});";
                    case "List<uint>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteUInt32List)}({paramSuffix});";
                    case "long": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteInt64)}({paramSuffix});";
                    case "long[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteInt64Arr)}({paramSuffix});";
                    case "List<long>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteInt64List)}({paramSuffix});";
                    case "ulong": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteUInt64)}({paramSuffix});";
                    case "ulong[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteUInt64Arr)}({paramSuffix});";
                    case "List<ulong>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteUInt64List)}({paramSuffix});";
                    case "float": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteSingle)}({paramSuffix});";
                    case "float[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteSingleArr)}({paramSuffix});";
                    case "List<float>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteSingleList)}({paramSuffix});";
                    case "double": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteDouble)}({paramSuffix});";
                    case "double[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteDoubleArr)}({paramSuffix});";
                    case "List<double>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteDoubleList)}({paramSuffix});";
                    case "decimal": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteDecimal)}({paramSuffix});";
                    case "decimal[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteDecimalArr)}({paramSuffix});";
                    case "List<decimal>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteDecimalList)}({paramSuffix});";
                    case "char": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteChar)}({paramSuffix});";
                    case "string": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteUTF8String)}({paramSuffix});";
                    case "string[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteUTF8StringArr)}({paramSuffix});";
                    case "List<string>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteUTF8StringList)}({paramSuffix});";
#if UNITY_EDITOR
                    case "Vector2": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteVector2)}({paramSuffix});";
                    case "Vector2[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteVector2Array)}({paramSuffix});";
                    case "List<Vector2>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteVector2List)}({paramSuffix});";
                    case "Vector2Int": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteVector2Int)}({paramSuffix});";
                    case "Vector2Int[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteVector2IntArray)}({paramSuffix});";
                    case "List<Vector2Int>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteVector2IntList)}({paramSuffix});";
                    case "Vector3": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteVector3)}({paramSuffix});";
                    case "Vector3[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteVector3Array)}({paramSuffix});";
                    case "List<Vector3>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteVector3List)}({paramSuffix});";
                    case "Vector3Int": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteVector3Int)}({paramSuffix});";
                    case "Vector3Int[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteVector3IntArray)}({paramSuffix});";
                    case "List<Vector3Int>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteVector3IntList)}({paramSuffix});";
                    case "Vector4": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteVector4)}({paramSuffix});";
                    case "Vector4[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteVector4Array)}({paramSuffix});";
                    case "List<Vector4>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteVector4List)}({paramSuffix});";
                    case "Quaternion": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteQuaternion)}({paramSuffix});";
                    case "Quaternion[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteQuaternionArray)}({paramSuffix});";
                    case "List<Quaternion>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteQuaternionList)}({paramSuffix});";
                    case "Color": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteColor)}({paramSuffix});";
                    case "Color[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteColorArray)}({paramSuffix});";
                    case "List<Color>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteColorList)}({paramSuffix});";
                    case "Color32": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteColor32)}({paramSuffix});";
                    case "Color32[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteColor32Array)}({paramSuffix});";
                    case "List<Color32>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteColor32List)}({paramSuffix});";
                    case "Rect": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteRect)}({paramSuffix});";
                    case "Rect[]": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteRectArray)}({paramSuffix});";
                    case "List<Rect>": return $"{nameof(GFBufferEncoderWriter)}.{nameof(GFBufferEncoderWriter.WriteRectList)}({paramSuffix});";
#endif
                    default:
                        if (t_field.EndsWith("[]>")) {
                            throw new Exception($"未处理该类型: {t_field}");
                        }
                        string className = classEditor.GetClassName();
                        if (t_field == className || t_field == $"{className}[]" || t_field == $"List<{className}>") {
                            throw new Exception($"不可循环依赖: {t_field}");
                        }

                        const string n_BufferWriterExtra = nameof(GFBufferEncoderWriter);
                        if (t_field.EndsWith("[]")) {
                            string trueType = t_field.Replace("[]", "");
                            if (IsBufferObject(inputDir, trueType)) {
                                const string n_WriteMessageArr = nameof(GFBufferEncoderWriter.WriteMessageArr);
                                return $"{n_BufferWriterExtra}.{n_WriteMessageArr}({paramSuffix});";
                            } else {
                                throw new Exception($"未处理该类型: {t_field}");
                            }
                        } else if (t_field.StartsWith("List<") && t_field.EndsWith(">")) {
                            string trueType = t_field.Replace("List<", "").Replace(">", "");
                            if (IsBufferObject(inputDir, trueType)) {
                                const string n_WriteMessageList = nameof(GFBufferEncoderWriter.WriteMessageList);
                                return $"{n_BufferWriterExtra}.{n_WriteMessageList}({paramSuffix});";
                            } else {
                                throw new Exception($"未处理该类型: {t_field}");
                            }
                        } else {
                            if (IsBufferObject(inputDir, t_field)) {
                                const string n_WriteMessage = nameof(GFBufferEncoderWriter.WriteMessage);
                                return $"{n_BufferWriterExtra}.{n_WriteMessage}({paramSuffix});";
                            } else {
                                throw new Exception($"未处理该类型: {t_field}");
                            }
                        }

                }
            }

            MethodEditor methodEditor = new MethodEditor();
            methodEditor.Initialize(VisitLevel.Public, false, "void", n_WriteTo);
            methodEditor.AddParameter(t_dst, n_dst);
            methodEditor.AddParameter(t_ref_int, n_offset);
            var fieldList = classEditor.GetAllFields();
            for (int i = 0; i < fieldList.Count; i += 1) {
                var field = fieldList[i];
                string type = field.GetFieldType();
                string name = field.GetFieldName();
                string line = WriteLine(type, name);
                methodEditor.AppendLine(line);
            }
            return methodEditor;
        }

        static MethodEditor GenFromBytesMethod(string inputDir, GFClassEditor classEditor) {
            const string t_src = "byte[]";
            const string n_src = "src";
            const string t_ref_int = "ref int";
            const string n_offset = "offset";

            string WriteLine(string t_field, string n_field) {
                const string n_BufferReader = nameof(GFBufferEncoderReader);
                string paramSuffix = $"{n_src}, ref {n_offset}";
                switch (t_field) {
                    case "byte": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadUInt8)}({paramSuffix});";
                    case "byte[]": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadUInt8Arr)}({paramSuffix});";
                    case "List<byte>": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadUInt8List)}({paramSuffix});";
                    case "sbyte": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadInt8)}({paramSuffix});";
                    case "sbyte[]": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadInt8Arr)}({paramSuffix});";
                    case "List<sbyte>": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadInt8List)}({paramSuffix});";
                    case "bool": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadBool)}({paramSuffix});";
                    case "bool[]": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadBoolArr)}({paramSuffix});";
                    case "List<bool>": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadBoolList)}({paramSuffix});";
                    case "short": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadInt16)}({paramSuffix});";
                    case "short[]": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadInt16Arr)}({paramSuffix});";
                    case "List<short>": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadInt16List)}({paramSuffix});";
                    case "ushort": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadUInt16)}({paramSuffix});";
                    case "ushort[]": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadUInt16Arr)}({paramSuffix});";
                    case "List<ushort>": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadUInt16List)}({paramSuffix});";
                    case "int": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadInt32)}({paramSuffix});";
                    case "int[]": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadInt32Arr)}({paramSuffix});";
                    case "List<int>": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadInt32List)}({paramSuffix});";
                    case "uint": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadUInt32)}({paramSuffix});";
                    case "uint[]": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadUInt32Arr)}({paramSuffix});";
                    case "List<uint>": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadUInt32List)}({paramSuffix});";
                    case "long": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadInt64)}({paramSuffix});";
                    case "long[]": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadInt64Arr)}({paramSuffix});";
                    case "List<long>": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadInt64List)}({paramSuffix});";
                    case "ulong": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadUInt64)}({paramSuffix});";
                    case "ulong[]": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadUInt64Arr)}({paramSuffix});";
                    case "List<ulong>": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadUInt64List)}({paramSuffix});";
                    case "float": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadSingle)}({paramSuffix});";
                    case "float[]": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadSingleArr)}({paramSuffix});";
                    case "List<float>": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadSingleList)}({paramSuffix});";
                    case "double": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadDouble)}({paramSuffix});";
                    case "double[]": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadDoubleArr)}({paramSuffix});";
                    case "List<double>": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadDoubleList)}({paramSuffix});";
                    case "decimal": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadDecimal)}({paramSuffix});";
                    case "decimal[]": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadDecimalArr)}({paramSuffix});";
                    case "List<decimal>": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadDecimalList)}({paramSuffix});";
                    case "char": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadChar)}({paramSuffix});";
                    case "string": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadUTF8String)}({paramSuffix});";
                    case "string[]": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadUTF8StringArr)}({paramSuffix});";
                    case "List<string>": return $"{n_field} = {n_BufferReader}.{nameof(GFBufferEncoderReader.ReadUTF8StringList)}({paramSuffix});";
#if UNITY_EDITOR
                    case "Vector2": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadVector2)}({paramSuffix});";
                    case "Vector2[]": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadVector2Array)}({paramSuffix});";
                    case "List<Vector2>": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadVector2List)}({paramSuffix});";
                    case "Vector2Int": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadVector2Int)}({paramSuffix});";
                    case "Vector2Int[]": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadVector2IntArray)}({paramSuffix});";
                    case "List<Vector2Int>": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadVector2IntList)}({paramSuffix});";
                    case "Vector3": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadVector3)}({paramSuffix});";
                    case "Vector3[]": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadVector3Array)}({paramSuffix});";
                    case "List<Vector3>": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadVector3List)}({paramSuffix});";
                    case "Vector3Int": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadVector3Int)}({paramSuffix});";
                    case "Vector3Int[]": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadVector3IntArray)}({paramSuffix});";
                    case "List<Vector3Int>": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadVector3IntList)}({paramSuffix});";
                    case "Vector4": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadVector4)}({paramSuffix});";
                    case "Vector4[]": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadVector4Array)}({paramSuffix});";
                    case "List<Vector4>": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadVector4List)}({paramSuffix});";
                    case "Quaternion": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadQuaternion)}({paramSuffix});";
                    case "Quaternion[]": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadQuaternionArray)}({paramSuffix});";
                    case "List<Quaternion>": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadQuaternionList)}({paramSuffix});";
                    case "Color": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadColor)}({paramSuffix});";
                    case "Color[]": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadColorArray)}({paramSuffix});";
                    case "List<Color>": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadColorList)}({paramSuffix});";
                    case "Color32": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadColor32)}({paramSuffix});";
                    case "Color32[]": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadColor32Array)}({paramSuffix});";
                    case "List<Color32>": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadColor32List)}({paramSuffix});";
                    case "Rect": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadRect)}({paramSuffix});";
                    case "Rect[]": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadRectArray)}({paramSuffix});";
                    case "List<Rect>": return $"{n_field} = {nameof(GFBufferEncoderReader)}.{nameof(GFBufferEncoderReader.ReadRectList)}({paramSuffix});";
#endif
                    default:
                        string className = classEditor.GetClassName();
                        if (t_field.EndsWith("[]>")) {
                            throw new Exception($"未处理该类型: {t_field}");
                        }
                        if (t_field == className || t_field == $"{className}[]" || t_field == $"List<{className}>") {
                            throw new Exception($"不可循环依赖: {t_field}");
                        }
                        const string n_BufferReaderExtra = nameof(GFBufferEncoderReader);
                        if (t_field.EndsWith("[]")) {
                            // 处理自定义类型数组
                            const string n_ReadMessageArr = nameof(GFBufferEncoderReader.ReadMessageArr);
                            string t_trueField = t_field.Replace("[]", "");
                            if (IsBufferObject(inputDir, t_trueField)) {
                                return $"{n_field} = {n_BufferReaderExtra}.{n_ReadMessageArr}({n_src}, () => new {t_trueField}(), ref {n_offset});";
                            } else {
                                throw new Exception($"未处理该类型: {t_field}");
                            }
                        } else if (t_field.StartsWith("List<") && t_field.EndsWith(">")) {
                            // 处理自定义类型列表
                            const string n_ReadMessageList = nameof(GFBufferEncoderReader.ReadMessageList);
                            string t_trueField = t_field.Replace("List<", "").Replace(">", "");
                            if (IsBufferObject(inputDir, t_trueField)) {
                                return $"{n_field} = {n_BufferReaderExtra}.{n_ReadMessageList}({n_src}, () => new {t_trueField}(), ref {n_offset});";
                            } else {
                                throw new Exception($"未处理该类型: {t_field}");
                            }
                        } else {
                            // 处理单自定义类型
                            const string n_ReadMessage = nameof(GFBufferEncoderReader.ReadMessage);
                            if (IsBufferObject(inputDir, t_field)) {
                                return $"{n_field} = {n_BufferReaderExtra}.{n_ReadMessage}({n_src}, () => new {t_field}(), ref {n_offset});";
                            } else {
                                throw new Exception($"未处理该类型: {t_field}");
                            }
                        }
                }
            }

            MethodEditor methodEditor = new MethodEditor();
            methodEditor.Initialize(VisitLevel.Public, false, "void", n_FromBytes);
            methodEditor.AddParameter(t_src, n_src);
            methodEditor.AddParameter(t_ref_int, n_offset);
            var fieldList = classEditor.GetAllFields();
            for (int i = 0; i < fieldList.Count; i += 1) {
                var field = fieldList[i];
                string type = field.GetFieldType();
                string name = field.GetFieldName();
                string checkLine = $"if ({n_src}.Length <= {n_offset}) return;";
                methodEditor.AppendLine(checkLine);
                string line = WriteLine(type, name);
                methodEditor.AppendLine(line);
            }
            return methodEditor;

        }

        static bool IsBufferObject(string inputDir, string fieldType) {
            string filePath = FindFileWithExt(inputDir, fieldType, "*.cs");
            if (string.IsNullOrEmpty(filePath)) {
                throw new Exception($"找不到代码文件: {fieldType}.cs");
            }

            GFClassEditor tarTypeClass = new GFClassEditor();
            tarTypeClass.LoadCode(File.ReadAllText(filePath));
            bool hasAttr = tarTypeClass.HasClassAttribute(ATTR);
            return hasAttr;
        }

        // 找到某个文件
        static string FindFileWithExt(string rootPath, string fileName, string ext) {
            List<string> all = FindAllFileWithExt(rootPath, ext);
            return all.Find(value => value.Contains(fileName + ext.TrimStart('*')));
        }

        // 递归
        static List<string> FindAllFileWithExt(string rootPath, string ext) {

            List<string> fileList = new List<string>();

            DirectoryInfo directoryInfo = new DirectoryInfo(rootPath);
            FileInfo[] allFiles = directoryInfo.GetFiles(ext);
            for (int i = 0; i < allFiles.Length; i += 1) {
                var file = allFiles[i];
                fileList.Add(file.FullName);
            }

            DirectoryInfo[] childrenDirs = directoryInfo.GetDirectories();
            for (int i = 0; i < childrenDirs.Length; i += 1) {
                var dir = childrenDirs[i];
                fileList.AddRange(FindAllFileWithExt(dir.FullName, ext));
            }

            return fileList;

        }

    }

}