using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Crypto.Digests
{
	public class SkeinEngine : IMemoable
	{
		private class Configuration
		{
			private byte[] bytes = new byte[32];

			public byte[] Bytes
			{
				get
				{
					return this.bytes;
				}
			}

			public Configuration(long outputSizeBits)
			{
				this.bytes[0] = 83;
				this.bytes[1] = 72;
				this.bytes[2] = 65;
				this.bytes[3] = 51;
				this.bytes[4] = 1;
				this.bytes[5] = 0;
				ThreefishEngine.WordToBytes((ulong)outputSizeBits, this.bytes, 8);
			}
		}

		public class Parameter
		{
			private int type;

			private byte[] value;

			public int Type
			{
				get
				{
					return this.type;
				}
			}

			public byte[] Value
			{
				get
				{
					return this.value;
				}
			}

			public Parameter(int type, byte[] value)
			{
				this.type = type;
				this.value = value;
			}
		}

		private class UbiTweak
		{
			private const ulong LOW_RANGE = 18446744069414584320uL;

			private const ulong T1_FINAL = 9223372036854775808uL;

			private const ulong T1_FIRST = 4611686018427387904uL;

			private ulong[] tweak = new ulong[2];

			private bool extendedPosition;

			public uint Type
			{
				get
				{
					return (uint)(this.tweak[1] >> 56 & 63uL);
				}
				set
				{
					this.tweak[1] = ((this.tweak[1] & 18446743798831644672uL) | ((ulong)value & 63uL) << 56);
				}
			}

			public bool First
			{
				get
				{
					return (this.tweak[1] & 4611686018427387904uL) != 0uL;
				}
				set
				{
					if (value)
					{
						this.tweak[1] |= 4611686018427387904uL;
						return;
					}
					this.tweak[1] &= 13835058055282163711uL;
				}
			}

			public bool Final
			{
				get
				{
					return (this.tweak[1] & 9223372036854775808uL) != 0uL;
				}
				set
				{
					if (value)
					{
						this.tweak[1] |= 9223372036854775808uL;
						return;
					}
					this.tweak[1] &= 9223372036854775807uL;
				}
			}

			public UbiTweak()
			{
				this.Reset();
			}

			public void Reset(SkeinEngine.UbiTweak tweak)
			{
				this.tweak = Arrays.Clone(tweak.tweak, this.tweak);
				this.extendedPosition = tweak.extendedPosition;
			}

			public void Reset()
			{
				this.tweak[0] = 0uL;
				this.tweak[1] = 0uL;
				this.extendedPosition = false;
				this.First = true;
			}

			public void AdvancePosition(int advance)
			{
				if (this.extendedPosition)
				{
					ulong[] array = new ulong[]
					{
						this.tweak[0] & (ulong)-1,
						this.tweak[0] >> 32 & (ulong)-1,
						this.tweak[1] & (ulong)-1
					};
					ulong num = (ulong)((long)advance);
					for (int i = 0; i < array.Length; i++)
					{
						num += array[i];
						array[i] = num;
						num >>= 32;
					}
					this.tweak[0] = ((array[1] & (ulong)-1) << 32 | (array[0] & (ulong)-1));
					this.tweak[1] = ((this.tweak[1] & 18446744069414584320uL) | (array[2] & (ulong)-1));
					return;
				}
				ulong num2 = this.tweak[0];
				num2 += (ulong)advance;
				this.tweak[0] = num2;
				if (num2 > 18446744069414584320uL)
				{
					this.extendedPosition = true;
				}
			}

			public ulong[] GetWords()
			{
				return this.tweak;
			}

			public override string ToString()
			{
				return string.Concat(new object[]
				{
					this.Type,
					" first: ",
					this.First,
					", final: ",
					this.Final
				});
			}
		}

		private class UBI
		{
			private readonly SkeinEngine.UbiTweak tweak = new SkeinEngine.UbiTweak();

			private readonly SkeinEngine engine;

			private byte[] currentBlock;

			private int currentOffset;

			private ulong[] message;

			public UBI(SkeinEngine engine, int blockSize)
			{
				this.engine = engine;
				this.currentBlock = new byte[blockSize];
				this.message = new ulong[this.currentBlock.Length / 8];
			}

			public void Reset(SkeinEngine.UBI ubi)
			{
				this.currentBlock = Arrays.Clone(ubi.currentBlock, this.currentBlock);
				this.currentOffset = ubi.currentOffset;
				this.message = Arrays.Clone(ubi.message, this.message);
				this.tweak.Reset(ubi.tweak);
			}

			public void Reset(int type)
			{
				this.tweak.Reset();
				this.tweak.Type = (uint)type;
				this.currentOffset = 0;
			}

			public void Update(byte[] value, int offset, int len, ulong[] output)
			{
				int num = 0;
				while (len > num)
				{
					if (this.currentOffset == this.currentBlock.Length)
					{
						this.ProcessBlock(output);
						this.tweak.First = false;
						this.currentOffset = 0;
					}
					int num2 = Math.Min(len - num, this.currentBlock.Length - this.currentOffset);
					Array.Copy(value, offset + num, this.currentBlock, this.currentOffset, num2);
					num += num2;
					this.currentOffset += num2;
					this.tweak.AdvancePosition(num2);
				}
			}

			private void ProcessBlock(ulong[] output)
			{
				this.engine.threefish.Init(true, this.engine.chain, this.tweak.GetWords());
				for (int i = 0; i < this.message.Length; i++)
				{
					this.message[i] = ThreefishEngine.BytesToWord(this.currentBlock, i * 8);
				}
				this.engine.threefish.ProcessBlock(this.message, output);
				for (int j = 0; j < output.Length; j++)
				{
					output[j] ^= this.message[j];
				}
			}

			public void DoFinal(ulong[] output)
			{
				for (int i = this.currentOffset; i < this.currentBlock.Length; i++)
				{
					this.currentBlock[i] = 0;
				}
				this.tweak.Final = true;
				this.ProcessBlock(output);
			}
		}

		public const int SKEIN_256 = 256;

		public const int SKEIN_512 = 512;

		public const int SKEIN_1024 = 1024;

		private const int PARAM_TYPE_KEY = 0;

		private const int PARAM_TYPE_CONFIG = 4;

		private const int PARAM_TYPE_MESSAGE = 48;

		private const int PARAM_TYPE_OUTPUT = 63;

		private static readonly IDictionary INITIAL_STATES;

		private readonly ThreefishEngine threefish;

		private readonly int outputSizeBytes;

		private ulong[] chain;

		private ulong[] initialState;

		private byte[] key;

		private SkeinEngine.Parameter[] preMessageParameters;

		private SkeinEngine.Parameter[] postMessageParameters;

		private readonly SkeinEngine.UBI ubi;

		private readonly byte[] singleByte = new byte[1];

		public int OutputSize
		{
			get
			{
				return this.outputSizeBytes;
			}
		}

		public int BlockSize
		{
			get
			{
				return this.threefish.GetBlockSize();
			}
		}

		static SkeinEngine()
		{
			SkeinEngine.INITIAL_STATES = Platform.CreateHashtable();
			SkeinEngine.InitialState(256, 128, new ulong[]
			{
				16217771249220022880uL,
				9817190399063458076uL,
				1155188648486244218uL,
				14769517481627992514uL
			});
			SkeinEngine.InitialState(256, 160, new ulong[]
			{
				1450197650740764312uL,
				3081844928540042640uL,
				15310647011875280446uL,
				3301952811952417661uL
			});
			SkeinEngine.InitialState(256, 224, new ulong[]
			{
				14270089230798940683uL,
				9758551101254474012uL,
				11082101768697755780uL,
				4056579644589979102uL
			});
			SkeinEngine.InitialState(256, 256, new ulong[]
			{
				18202890402666165321uL,
				3443677322885453875uL,
				12915131351309911055uL,
				7662005193972177513uL
			});
			SkeinEngine.InitialState(512, 128, new ulong[]
			{
				12158729379475595090uL,
				2204638249859346602uL,
				3502419045458743507uL,
				13617680570268287068uL,
				983504137758028059uL,
				1880512238245786339uL,
				11730851291495443074uL,
				7602827311880509485uL
			});
			SkeinEngine.InitialState(512, 160, new ulong[]
			{
				2934123928682216849uL,
				14047033351726823311uL,
				1684584802963255058uL,
				5744138295201861711uL,
				2444857010922934358uL,
				15638910433986703544uL,
				13325156239043941114uL,
				118355523173251694uL
			});
			SkeinEngine.InitialState(512, 224, new ulong[]
			{
				14758403053642543652uL,
				14674518637417806319uL,
				10145881904771976036uL,
				4146387520469897396uL,
				1106145742801415120uL,
				7455425944880474941uL,
				11095680972475339753uL,
				11397762726744039159uL
			});
			SkeinEngine.InitialState(512, 384, new ulong[]
			{
				11814849197074935647uL,
				12753905853581818532uL,
				11346781217370868990uL,
				15535391162178797018uL,
				2000907093792408677uL,
				9140007292425499655uL,
				6093301768906360022uL,
				2769176472213098488uL
			});
			SkeinEngine.InitialState(512, 512, new ulong[]
			{
				5261240102383538638uL,
				978932832955457283uL,
				10363226125605772238uL,
				11107378794354519217uL,
				6752626034097301424uL,
				16915020251879818228uL,
				11029617608758768931uL,
				12544957130904423475uL
			});
		}

		private static void InitialState(int blockSize, int outputSize, ulong[] state)
		{
			SkeinEngine.INITIAL_STATES.Add(SkeinEngine.VariantIdentifier(blockSize / 8, outputSize / 8), state);
		}

		private static int VariantIdentifier(int blockSizeBytes, int outputSizeBytes)
		{
			return outputSizeBytes << 16 | blockSizeBytes;
		}

		public SkeinEngine(int blockSizeBits, int outputSizeBits)
		{
			if (outputSizeBits % 8 != 0)
			{
				throw new ArgumentException("Output size must be a multiple of 8 bits. :" + outputSizeBits);
			}
			this.outputSizeBytes = outputSizeBits / 8;
			this.threefish = new ThreefishEngine(blockSizeBits);
			this.ubi = new SkeinEngine.UBI(this, this.threefish.GetBlockSize());
		}

		public SkeinEngine(SkeinEngine engine) : this(engine.BlockSize * 8, engine.OutputSize * 8)
		{
			this.CopyIn(engine);
		}

		private void CopyIn(SkeinEngine engine)
		{
			this.ubi.Reset(engine.ubi);
			this.chain = Arrays.Clone(engine.chain, this.chain);
			this.initialState = Arrays.Clone(engine.initialState, this.initialState);
			this.key = Arrays.Clone(engine.key, this.key);
			this.preMessageParameters = SkeinEngine.Clone(engine.preMessageParameters, this.preMessageParameters);
			this.postMessageParameters = SkeinEngine.Clone(engine.postMessageParameters, this.postMessageParameters);
		}

		private static SkeinEngine.Parameter[] Clone(SkeinEngine.Parameter[] data, SkeinEngine.Parameter[] existing)
		{
			if (data == null)
			{
				return null;
			}
			if (existing == null || existing.Length != data.Length)
			{
				existing = new SkeinEngine.Parameter[data.Length];
			}
			Array.Copy(data, 0, existing, 0, existing.Length);
			return existing;
		}

		public IMemoable Copy()
		{
			return new SkeinEngine(this);
		}

		public void Reset(IMemoable other)
		{
			SkeinEngine skeinEngine = (SkeinEngine)other;
			if (this.BlockSize != skeinEngine.BlockSize || this.outputSizeBytes != skeinEngine.outputSizeBytes)
			{
				throw new MemoableResetException("Incompatible parameters in provided SkeinEngine.");
			}
			this.CopyIn(skeinEngine);
		}

		public void Init(SkeinParameters parameters)
		{
			this.chain = null;
			this.key = null;
			this.preMessageParameters = null;
			this.postMessageParameters = null;
			if (parameters != null)
			{
				byte[] array = parameters.GetKey();
				if (array.Length < 16)
				{
					throw new ArgumentException("Skein key must be at least 128 bits.");
				}
				this.InitParams(parameters.GetParameters());
			}
			this.CreateInitialState();
			this.UbiInit(48);
		}

		private void InitParams(IDictionary parameters)
		{
			IEnumerator enumerator = parameters.Keys.GetEnumerator();
			IList list = Platform.CreateArrayList();
			IList list2 = Platform.CreateArrayList();
			while (enumerator.MoveNext())
			{
				int num = (int)enumerator.Current;
				byte[] value = (byte[])parameters[num];
				if (num == 0)
				{
					this.key = value;
				}
				else if (num < 48)
				{
					list.Add(new SkeinEngine.Parameter(num, value));
				}
				else
				{
					list2.Add(new SkeinEngine.Parameter(num, value));
				}
			}
			this.preMessageParameters = new SkeinEngine.Parameter[list.Count];
			list.CopyTo(this.preMessageParameters, 0);
			Array.Sort<SkeinEngine.Parameter>(this.preMessageParameters);
			this.postMessageParameters = new SkeinEngine.Parameter[list2.Count];
			list2.CopyTo(this.postMessageParameters, 0);
			Array.Sort<SkeinEngine.Parameter>(this.postMessageParameters);
		}

		private void CreateInitialState()
		{
			ulong[] array = (ulong[])SkeinEngine.INITIAL_STATES[SkeinEngine.VariantIdentifier(this.BlockSize, this.OutputSize)];
			if (this.key == null && array != null)
			{
				this.chain = Arrays.Clone(array);
			}
			else
			{
				this.chain = new ulong[this.BlockSize / 8];
				if (this.key != null)
				{
					this.UbiComplete(0, this.key);
				}
				this.UbiComplete(4, new SkeinEngine.Configuration((long)(this.outputSizeBytes * 8)).Bytes);
			}
			if (this.preMessageParameters != null)
			{
				for (int i = 0; i < this.preMessageParameters.Length; i++)
				{
					SkeinEngine.Parameter parameter = this.preMessageParameters[i];
					this.UbiComplete(parameter.Type, parameter.Value);
				}
			}
			this.initialState = Arrays.Clone(this.chain);
		}

		public void Reset()
		{
			Array.Copy(this.initialState, 0, this.chain, 0, this.chain.Length);
			this.UbiInit(48);
		}

		private void UbiComplete(int type, byte[] value)
		{
			this.UbiInit(type);
			this.ubi.Update(value, 0, value.Length, this.chain);
			this.UbiFinal();
		}

		private void UbiInit(int type)
		{
			this.ubi.Reset(type);
		}

		private void UbiFinal()
		{
			this.ubi.DoFinal(this.chain);
		}

		private void CheckInitialised()
		{
			if (this.ubi == null)
			{
				throw new ArgumentException("Skein engine is not initialised.");
			}
		}

		public void Update(byte inByte)
		{
			this.singleByte[0] = inByte;
			this.Update(this.singleByte, 0, 1);
		}

		public void Update(byte[] inBytes, int inOff, int len)
		{
			this.CheckInitialised();
			this.ubi.Update(inBytes, inOff, len, this.chain);
		}

		public int DoFinal(byte[] outBytes, int outOff)
		{
			this.CheckInitialised();
			if (outBytes.Length < outOff + this.outputSizeBytes)
			{
				throw new DataLengthException("Output buffer is too short to hold output of " + this.outputSizeBytes + " bytes");
			}
			this.UbiFinal();
			if (this.postMessageParameters != null)
			{
				for (int i = 0; i < this.postMessageParameters.Length; i++)
				{
					SkeinEngine.Parameter parameter = this.postMessageParameters[i];
					this.UbiComplete(parameter.Type, parameter.Value);
				}
			}
			int blockSize = this.BlockSize;
			int num = (this.outputSizeBytes + blockSize - 1) / blockSize;
			for (int j = 0; j < num; j++)
			{
				int outputBytes = Math.Min(blockSize, this.outputSizeBytes - j * blockSize);
				this.Output((ulong)((long)j), outBytes, outOff + j * blockSize, outputBytes);
			}
			this.Reset();
			return this.outputSizeBytes;
		}

		private void Output(ulong outputSequence, byte[] outBytes, int outOff, int outputBytes)
		{
			byte[] array = new byte[8];
			ThreefishEngine.WordToBytes(outputSequence, array, 0);
			ulong[] array2 = new ulong[this.chain.Length];
			this.UbiInit(63);
			this.ubi.Update(array, 0, array.Length, array2);
			this.ubi.DoFinal(array2);
			int num = (outputBytes + 8 - 1) / 8;
			for (int i = 0; i < num; i++)
			{
				int num2 = Math.Min(8, outputBytes - i * 8);
				if (num2 == 8)
				{
					ThreefishEngine.WordToBytes(array2[i], outBytes, outOff + i * 8);
				}
				else
				{
					ThreefishEngine.WordToBytes(array2[i], array, 0);
					Array.Copy(array, 0, outBytes, outOff + i * 8, num2);
				}
			}
		}
	}
}
