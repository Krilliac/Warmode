using System;

namespace Org.BouncyCastle.Utilities.Zlib
{
	public sealed class Deflate
	{
		internal class Config
		{
			internal int good_length;

			internal int max_lazy;

			internal int nice_length;

			internal int max_chain;

			internal int func;

			internal Config(int good_length, int max_lazy, int nice_length, int max_chain, int func)
			{
				this.good_length = good_length;
				this.max_lazy = max_lazy;
				this.nice_length = nice_length;
				this.max_chain = max_chain;
				this.func = func;
			}
		}

		private const int MAX_MEM_LEVEL = 9;

		private const int Z_DEFAULT_COMPRESSION = -1;

		private const int MAX_WBITS = 15;

		private const int DEF_MEM_LEVEL = 8;

		private const int STORED = 0;

		private const int FAST = 1;

		private const int SLOW = 2;

		private const int NeedMore = 0;

		private const int BlockDone = 1;

		private const int FinishStarted = 2;

		private const int FinishDone = 3;

		private const int PRESET_DICT = 32;

		private const int Z_FILTERED = 1;

		private const int Z_HUFFMAN_ONLY = 2;

		private const int Z_DEFAULT_STRATEGY = 0;

		private const int Z_NO_FLUSH = 0;

		private const int Z_PARTIAL_FLUSH = 1;

		private const int Z_SYNC_FLUSH = 2;

		private const int Z_FULL_FLUSH = 3;

		private const int Z_FINISH = 4;

		private const int Z_OK = 0;

		private const int Z_STREAM_END = 1;

		private const int Z_NEED_DICT = 2;

		private const int Z_ERRNO = -1;

		private const int Z_STREAM_ERROR = -2;

		private const int Z_DATA_ERROR = -3;

		private const int Z_MEM_ERROR = -4;

		private const int Z_BUF_ERROR = -5;

		private const int Z_VERSION_ERROR = -6;

		private const int INIT_STATE = 42;

		private const int BUSY_STATE = 113;

		private const int FINISH_STATE = 666;

		private const int Z_DEFLATED = 8;

		private const int STORED_BLOCK = 0;

		private const int STATIC_TREES = 1;

		private const int DYN_TREES = 2;

		private const int Z_BINARY = 0;

		private const int Z_ASCII = 1;

		private const int Z_UNKNOWN = 2;

		private const int Buf_size = 16;

		private const int REP_3_6 = 16;

		private const int REPZ_3_10 = 17;

		private const int REPZ_11_138 = 18;

		private const int MIN_MATCH = 3;

		private const int MAX_MATCH = 258;

		private const int MIN_LOOKAHEAD = 262;

		private const int MAX_BITS = 15;

		private const int D_CODES = 30;

		private const int BL_CODES = 19;

		private const int LENGTH_CODES = 29;

		private const int LITERALS = 256;

		private const int L_CODES = 286;

		private const int HEAP_SIZE = 573;

		private const int END_BLOCK = 256;

		private static readonly Deflate.Config[] config_table;

		private static readonly string[] z_errmsg;

		internal ZStream strm;

		internal int status;

		internal byte[] pending_buf;

		internal int pending_buf_size;

		internal int pending_out;

		internal int pending;

		internal int noheader;

		internal byte data_type;

		internal byte method;

		internal int last_flush;

		internal int w_size;

		internal int w_bits;

		internal int w_mask;

		internal byte[] window;

		internal int window_size;

		internal short[] prev;

		internal short[] head;

		internal int ins_h;

		internal int hash_size;

		internal int hash_bits;

		internal int hash_mask;

		internal int hash_shift;

		internal int block_start;

		internal int match_length;

		internal int prev_match;

		internal int match_available;

		internal int strstart;

		internal int match_start;

		internal int lookahead;

		internal int prev_length;

		internal int max_chain_length;

		internal int max_lazy_match;

		internal int level;

		internal int strategy;

		internal int good_match;

		internal int nice_match;

		internal short[] dyn_ltree;

		internal short[] dyn_dtree;

		internal short[] bl_tree;

		internal Tree l_desc = new Tree();

		internal Tree d_desc = new Tree();

		internal Tree bl_desc = new Tree();

		internal short[] bl_count = new short[16];

		internal int[] heap = new int[573];

		internal int heap_len;

		internal int heap_max;

		internal byte[] depth = new byte[573];

		internal int l_buf;

		internal int lit_bufsize;

		internal int last_lit;

		internal int d_buf;

		internal int opt_len;

		internal int static_len;

		internal int matches;

		internal int last_eob_len;

		internal uint bi_buf;

		internal int bi_valid;

		static Deflate()
		{
			Deflate.z_errmsg = new string[]
			{
				"need dictionary",
				"stream end",
				"",
				"file error",
				"stream error",
				"data error",
				"insufficient memory",
				"buffer error",
				"incompatible version",
				""
			};
			Deflate.config_table = new Deflate.Config[10];
			Deflate.config_table[0] = new Deflate.Config(0, 0, 0, 0, 0);
			Deflate.config_table[1] = new Deflate.Config(4, 4, 8, 4, 1);
			Deflate.config_table[2] = new Deflate.Config(4, 5, 16, 8, 1);
			Deflate.config_table[3] = new Deflate.Config(4, 6, 32, 32, 1);
			Deflate.config_table[4] = new Deflate.Config(4, 4, 16, 16, 2);
			Deflate.config_table[5] = new Deflate.Config(8, 16, 32, 32, 2);
			Deflate.config_table[6] = new Deflate.Config(8, 16, 128, 128, 2);
			Deflate.config_table[7] = new Deflate.Config(8, 32, 128, 256, 2);
			Deflate.config_table[8] = new Deflate.Config(32, 128, 258, 1024, 2);
			Deflate.config_table[9] = new Deflate.Config(32, 258, 258, 4096, 2);
		}

		internal Deflate()
		{
			this.dyn_ltree = new short[1146];
			this.dyn_dtree = new short[122];
			this.bl_tree = new short[78];
		}

		internal void lm_init()
		{
			this.window_size = 2 * this.w_size;
			this.head[this.hash_size - 1] = 0;
			for (int i = 0; i < this.hash_size - 1; i++)
			{
				this.head[i] = 0;
			}
			this.max_lazy_match = Deflate.config_table[this.level].max_lazy;
			this.good_match = Deflate.config_table[this.level].good_length;
			this.nice_match = Deflate.config_table[this.level].nice_length;
			this.max_chain_length = Deflate.config_table[this.level].max_chain;
			this.strstart = 0;
			this.block_start = 0;
			this.lookahead = 0;
			this.match_length = (this.prev_length = 2);
			this.match_available = 0;
			this.ins_h = 0;
		}

		internal void tr_init()
		{
			this.l_desc.dyn_tree = this.dyn_ltree;
			this.l_desc.stat_desc = StaticTree.static_l_desc;
			this.d_desc.dyn_tree = this.dyn_dtree;
			this.d_desc.stat_desc = StaticTree.static_d_desc;
			this.bl_desc.dyn_tree = this.bl_tree;
			this.bl_desc.stat_desc = StaticTree.static_bl_desc;
			this.bi_buf = 0u;
			this.bi_valid = 0;
			this.last_eob_len = 8;
			this.init_block();
		}

		internal void init_block()
		{
			for (int i = 0; i < 286; i++)
			{
				this.dyn_ltree[i * 2] = 0;
			}
			for (int j = 0; j < 30; j++)
			{
				this.dyn_dtree[j * 2] = 0;
			}
			for (int k = 0; k < 19; k++)
			{
				this.bl_tree[k * 2] = 0;
			}
			this.dyn_ltree[512] = 1;
			this.opt_len = (this.static_len = 0);
			this.last_lit = (this.matches = 0);
		}

		internal void pqdownheap(short[] tree, int k)
		{
			int num = this.heap[k];
			for (int i = k << 1; i <= this.heap_len; i <<= 1)
			{
				if (i < this.heap_len && Deflate.smaller(tree, this.heap[i + 1], this.heap[i], this.depth))
				{
					i++;
				}
				if (Deflate.smaller(tree, num, this.heap[i], this.depth))
				{
					break;
				}
				this.heap[k] = this.heap[i];
				k = i;
			}
			this.heap[k] = num;
		}

		internal static bool smaller(short[] tree, int n, int m, byte[] depth)
		{
			short num = tree[n * 2];
			short num2 = tree[m * 2];
			return num < num2 || (num == num2 && depth[n] <= depth[m]);
		}

		internal void scan_tree(short[] tree, int max_code)
		{
			int num = -1;
			int num2 = (int)tree[1];
			int num3 = 0;
			int num4 = 7;
			int num5 = 4;
			if (num2 == 0)
			{
				num4 = 138;
				num5 = 3;
			}
			tree[(max_code + 1) * 2 + 1] = -1;
			for (int i = 0; i <= max_code; i++)
			{
				int num6 = num2;
				num2 = (int)tree[(i + 1) * 2 + 1];
				if (++num3 >= num4 || num6 != num2)
				{
					if (num3 < num5)
					{
						short[] expr_5F_cp_0 = this.bl_tree;
						int expr_5F_cp_1 = num6 * 2;
						expr_5F_cp_0[expr_5F_cp_1] += (short)num3;
					}
					else if (num6 != 0)
					{
						if (num6 != num)
						{
							short[] expr_86_cp_0 = this.bl_tree;
							int expr_86_cp_1 = num6 * 2;
							expr_86_cp_0[expr_86_cp_1] += 1;
						}
						short[] expr_A1_cp_0 = this.bl_tree;
						int expr_A1_cp_1 = 32;
						expr_A1_cp_0[expr_A1_cp_1] += 1;
					}
					else if (num3 <= 10)
					{
						short[] expr_C4_cp_0 = this.bl_tree;
						int expr_C4_cp_1 = 34;
						expr_C4_cp_0[expr_C4_cp_1] += 1;
					}
					else
					{
						short[] expr_E1_cp_0 = this.bl_tree;
						int expr_E1_cp_1 = 36;
						expr_E1_cp_0[expr_E1_cp_1] += 1;
					}
					num3 = 0;
					num = num6;
					if (num2 == 0)
					{
						num4 = 138;
						num5 = 3;
					}
					else if (num6 == num2)
					{
						num4 = 6;
						num5 = 3;
					}
					else
					{
						num4 = 7;
						num5 = 4;
					}
				}
			}
		}

		internal int build_bl_tree()
		{
			this.scan_tree(this.dyn_ltree, this.l_desc.max_code);
			this.scan_tree(this.dyn_dtree, this.d_desc.max_code);
			this.bl_desc.build_tree(this);
			int num = 18;
			while (num >= 3 && this.bl_tree[(int)(Tree.bl_order[num] * 2 + 1)] == 0)
			{
				num--;
			}
			this.opt_len += 3 * (num + 1) + 5 + 5 + 4;
			return num;
		}

		internal void send_all_trees(int lcodes, int dcodes, int blcodes)
		{
			this.send_bits(lcodes - 257, 5);
			this.send_bits(dcodes - 1, 5);
			this.send_bits(blcodes - 4, 4);
			for (int i = 0; i < blcodes; i++)
			{
				this.send_bits((int)this.bl_tree[(int)(Tree.bl_order[i] * 2 + 1)], 3);
			}
			this.send_tree(this.dyn_ltree, lcodes - 1);
			this.send_tree(this.dyn_dtree, dcodes - 1);
		}

		internal void send_tree(short[] tree, int max_code)
		{
			int num = -1;
			int num2 = (int)tree[1];
			int num3 = 0;
			int num4 = 7;
			int num5 = 4;
			if (num2 == 0)
			{
				num4 = 138;
				num5 = 3;
			}
			for (int i = 0; i <= max_code; i++)
			{
				int num6 = num2;
				num2 = (int)tree[(i + 1) * 2 + 1];
				if (++num3 >= num4 || num6 != num2)
				{
					if (num3 < num5)
					{
						do
						{
							this.send_code(num6, this.bl_tree);
						}
						while (--num3 != 0);
					}
					else if (num6 != 0)
					{
						if (num6 != num)
						{
							this.send_code(num6, this.bl_tree);
							num3--;
						}
						this.send_code(16, this.bl_tree);
						this.send_bits(num3 - 3, 2);
					}
					else if (num3 <= 10)
					{
						this.send_code(17, this.bl_tree);
						this.send_bits(num3 - 3, 3);
					}
					else
					{
						this.send_code(18, this.bl_tree);
						this.send_bits(num3 - 11, 7);
					}
					num3 = 0;
					num = num6;
					if (num2 == 0)
					{
						num4 = 138;
						num5 = 3;
					}
					else if (num6 == num2)
					{
						num4 = 6;
						num5 = 3;
					}
					else
					{
						num4 = 7;
						num5 = 4;
					}
				}
			}
		}

		internal void put_byte(byte[] p, int start, int len)
		{
			Array.Copy(p, start, this.pending_buf, this.pending, len);
			this.pending += len;
		}

		internal void put_byte(byte c)
		{
			this.pending_buf[this.pending++] = c;
		}

		internal void put_short(int w)
		{
			this.pending_buf[this.pending++] = (byte)w;
			this.pending_buf[this.pending++] = (byte)(w >> 8);
		}

		internal void putShortMSB(int b)
		{
			this.pending_buf[this.pending++] = (byte)(b >> 8);
			this.pending_buf[this.pending++] = (byte)b;
		}

		internal void send_code(int c, short[] tree)
		{
			int num = c * 2;
			this.send_bits((int)tree[num] & 65535, (int)tree[num + 1] & 65535);
		}

		internal void send_bits(int val, int length)
		{
			if (this.bi_valid > 16 - length)
			{
				this.bi_buf |= (uint)((uint)val << this.bi_valid);
				this.pending_buf[this.pending++] = (byte)this.bi_buf;
				this.pending_buf[this.pending++] = (byte)(this.bi_buf >> 8);
				this.bi_buf = (uint)val >> 16 - this.bi_valid;
				this.bi_valid += length - 16;
				return;
			}
			this.bi_buf |= (uint)((uint)val << this.bi_valid);
			this.bi_valid += length;
		}

		internal void _tr_align()
		{
			this.send_bits(2, 3);
			this.send_code(256, StaticTree.static_ltree);
			this.bi_flush();
			if (1 + this.last_eob_len + 10 - this.bi_valid < 9)
			{
				this.send_bits(2, 3);
				this.send_code(256, StaticTree.static_ltree);
				this.bi_flush();
			}
			this.last_eob_len = 7;
		}

		internal bool _tr_tally(int dist, int lc)
		{
			this.pending_buf[this.d_buf + this.last_lit * 2] = (byte)(dist >> 8);
			this.pending_buf[this.d_buf + this.last_lit * 2 + 1] = (byte)dist;
			this.pending_buf[this.l_buf + this.last_lit] = (byte)lc;
			this.last_lit++;
			if (dist == 0)
			{
				short[] expr_69_cp_0 = this.dyn_ltree;
				int expr_69_cp_1 = lc * 2;
				expr_69_cp_0[expr_69_cp_1] += 1;
			}
			else
			{
				this.matches++;
				dist--;
				short[] expr_A8_cp_0 = this.dyn_ltree;
				int expr_A8_cp_1 = ((int)Tree._length_code[lc] + 256 + 1) * 2;
				expr_A8_cp_0[expr_A8_cp_1] += 1;
				short[] expr_C9_cp_0 = this.dyn_dtree;
				int expr_C9_cp_1 = Tree.d_code(dist) * 2;
				expr_C9_cp_0[expr_C9_cp_1] += 1;
			}
			if ((this.last_lit & 8191) == 0 && this.level > 2)
			{
				int num = this.last_lit * 8;
				int num2 = this.strstart - this.block_start;
				for (int i = 0; i < 30; i++)
				{
					num += (int)((long)this.dyn_dtree[i * 2] * (5L + (long)Tree.extra_dbits[i]));
				}
				num >>= 3;
				if (this.matches < this.last_lit / 2 && num < num2 / 2)
				{
					return true;
				}
			}
			return this.last_lit == this.lit_bufsize - 1;
		}

		internal void compress_block(short[] ltree, short[] dtree)
		{
			int num = 0;
			if (this.last_lit != 0)
			{
				do
				{
					int num2 = ((int)this.pending_buf[this.d_buf + num * 2] << 8 & 65280) | (int)(this.pending_buf[this.d_buf + num * 2 + 1] & 255);
					int num3 = (int)(this.pending_buf[this.l_buf + num] & 255);
					num++;
					if (num2 == 0)
					{
						this.send_code(num3, ltree);
					}
					else
					{
						int num4 = (int)Tree._length_code[num3];
						this.send_code(num4 + 256 + 1, ltree);
						int num5 = Tree.extra_lbits[num4];
						if (num5 != 0)
						{
							num3 -= Tree.base_length[num4];
							this.send_bits(num3, num5);
						}
						num2--;
						num4 = Tree.d_code(num2);
						this.send_code(num4, dtree);
						num5 = Tree.extra_dbits[num4];
						if (num5 != 0)
						{
							num2 -= Tree.base_dist[num4];
							this.send_bits(num2, num5);
						}
					}
				}
				while (num < this.last_lit);
			}
			this.send_code(256, ltree);
			this.last_eob_len = (int)ltree[513];
		}

		internal void set_data_type()
		{
			int i = 0;
			int num = 0;
			int num2 = 0;
			while (i < 7)
			{
				num2 += (int)this.dyn_ltree[i * 2];
				i++;
			}
			while (i < 128)
			{
				num += (int)this.dyn_ltree[i * 2];
				i++;
			}
			while (i < 256)
			{
				num2 += (int)this.dyn_ltree[i * 2];
				i++;
			}
			this.data_type = ((num2 > num >> 2) ? 0 : 1);
		}

		internal void bi_flush()
		{
			if (this.bi_valid == 16)
			{
				this.pending_buf[this.pending++] = (byte)this.bi_buf;
				this.pending_buf[this.pending++] = (byte)(this.bi_buf >> 8);
				this.bi_buf = 0u;
				this.bi_valid = 0;
				return;
			}
			if (this.bi_valid >= 8)
			{
				this.pending_buf[this.pending++] = (byte)this.bi_buf;
				this.bi_buf >>= 8;
				this.bi_buf &= 255u;
				this.bi_valid -= 8;
			}
		}

		internal void bi_windup()
		{
			if (this.bi_valid > 8)
			{
				this.pending_buf[this.pending++] = (byte)this.bi_buf;
				this.pending_buf[this.pending++] = (byte)(this.bi_buf >> 8);
			}
			else if (this.bi_valid > 0)
			{
				this.pending_buf[this.pending++] = (byte)this.bi_buf;
			}
			this.bi_buf = 0u;
			this.bi_valid = 0;
		}

		internal void copy_block(int buf, int len, bool header)
		{
			this.bi_windup();
			this.last_eob_len = 8;
			if (header)
			{
				this.put_short((int)((short)len));
				this.put_short((int)((short)(~(short)len)));
			}
			this.put_byte(this.window, buf, len);
		}

		internal void flush_block_only(bool eof)
		{
			this._tr_flush_block((this.block_start >= 0) ? this.block_start : -1, this.strstart - this.block_start, eof);
			this.block_start = this.strstart;
			this.strm.flush_pending();
		}

		internal int deflate_stored(int flush)
		{
			int num = 65535;
			if (num > this.pending_buf_size - 5)
			{
				num = this.pending_buf_size - 5;
			}
			while (true)
			{
				if (this.lookahead <= 1)
				{
					this.fill_window();
					if (this.lookahead == 0 && flush == 0)
					{
						break;
					}
					if (this.lookahead == 0)
					{
						goto IL_D7;
					}
				}
				this.strstart += this.lookahead;
				this.lookahead = 0;
				int num2 = this.block_start + num;
				if (this.strstart == 0 || this.strstart >= num2)
				{
					this.lookahead = this.strstart - num2;
					this.strstart = num2;
					this.flush_block_only(false);
					if (this.strm.avail_out == 0)
					{
						return 0;
					}
				}
				if (this.strstart - this.block_start >= this.w_size - 262)
				{
					this.flush_block_only(false);
					if (this.strm.avail_out == 0)
					{
						return 0;
					}
				}
			}
			return 0;
			IL_D7:
			this.flush_block_only(flush == 4);
			if (this.strm.avail_out == 0)
			{
				if (flush != 4)
				{
					return 0;
				}
				return 2;
			}
			else
			{
				if (flush != 4)
				{
					return 1;
				}
				return 3;
			}
		}

		internal void _tr_stored_block(int buf, int stored_len, bool eof)
		{
			this.send_bits(eof ? 1 : 0, 3);
			this.copy_block(buf, stored_len, true);
		}

		internal void _tr_flush_block(int buf, int stored_len, bool eof)
		{
			int num = 0;
			int num2;
			int num3;
			if (this.level > 0)
			{
				if (this.data_type == 2)
				{
					this.set_data_type();
				}
				this.l_desc.build_tree(this);
				this.d_desc.build_tree(this);
				num = this.build_bl_tree();
				num2 = this.opt_len + 3 + 7 >> 3;
				num3 = this.static_len + 3 + 7 >> 3;
				if (num3 <= num2)
				{
					num2 = num3;
				}
			}
			else
			{
				num3 = (num2 = stored_len + 5);
			}
			if (stored_len + 4 <= num2 && buf != -1)
			{
				this._tr_stored_block(buf, stored_len, eof);
			}
			else if (num3 == num2)
			{
				this.send_bits(2 + (eof ? 1 : 0), 3);
				this.compress_block(StaticTree.static_ltree, StaticTree.static_dtree);
			}
			else
			{
				this.send_bits(4 + (eof ? 1 : 0), 3);
				this.send_all_trees(this.l_desc.max_code + 1, this.d_desc.max_code + 1, num + 1);
				this.compress_block(this.dyn_ltree, this.dyn_dtree);
			}
			this.init_block();
			if (eof)
			{
				this.bi_windup();
			}
		}

		internal void fill_window()
		{
			while (true)
			{
				int num = this.window_size - this.lookahead - this.strstart;
				int num2;
				if (num == 0 && this.strstart == 0 && this.lookahead == 0)
				{
					num = this.w_size;
				}
				else if (num == -1)
				{
					num--;
				}
				else if (this.strstart >= this.w_size + this.w_size - 262)
				{
					Array.Copy(this.window, this.w_size, this.window, 0, this.w_size);
					this.match_start -= this.w_size;
					this.strstart -= this.w_size;
					this.block_start -= this.w_size;
					num2 = this.hash_size;
					int num3 = num2;
					do
					{
						int num4 = (int)this.head[--num3] & 65535;
						this.head[num3] = (short)((num4 >= this.w_size) ? (num4 - this.w_size) : 0);
					}
					while (--num2 != 0);
					num2 = this.w_size;
					num3 = num2;
					do
					{
						int num4 = (int)this.prev[--num3] & 65535;
						this.prev[num3] = (short)((num4 >= this.w_size) ? (num4 - this.w_size) : 0);
					}
					while (--num2 != 0);
					num += this.w_size;
				}
				if (this.strm.avail_in == 0)
				{
					break;
				}
				num2 = this.strm.read_buf(this.window, this.strstart + this.lookahead, num);
				this.lookahead += num2;
				if (this.lookahead >= 3)
				{
					this.ins_h = (int)(this.window[this.strstart] & 255);
					this.ins_h = ((this.ins_h << this.hash_shift ^ (int)(this.window[this.strstart + 1] & 255)) & this.hash_mask);
				}
				if (this.lookahead >= 262 || this.strm.avail_in == 0)
				{
					return;
				}
			}
		}

		internal int deflate_fast(int flush)
		{
			int num = 0;
			while (true)
			{
				if (this.lookahead < 262)
				{
					this.fill_window();
					if (this.lookahead < 262 && flush == 0)
					{
						break;
					}
					if (this.lookahead == 0)
					{
						goto IL_2C6;
					}
				}
				if (this.lookahead >= 3)
				{
					this.ins_h = ((this.ins_h << this.hash_shift ^ (int)(this.window[this.strstart + 2] & 255)) & this.hash_mask);
					num = ((int)this.head[this.ins_h] & 65535);
					this.prev[this.strstart & this.w_mask] = this.head[this.ins_h];
					this.head[this.ins_h] = (short)this.strstart;
				}
				if ((long)num != 0L && (this.strstart - num & 65535) <= this.w_size - 262 && this.strategy != 2)
				{
					this.match_length = this.longest_match(num);
				}
				bool flag;
				if (this.match_length >= 3)
				{
					flag = this._tr_tally(this.strstart - this.match_start, this.match_length - 3);
					this.lookahead -= this.match_length;
					if (this.match_length <= this.max_lazy_match && this.lookahead >= 3)
					{
						this.match_length--;
						do
						{
							this.strstart++;
							this.ins_h = ((this.ins_h << this.hash_shift ^ (int)(this.window[this.strstart + 2] & 255)) & this.hash_mask);
							num = ((int)this.head[this.ins_h] & 65535);
							this.prev[this.strstart & this.w_mask] = this.head[this.ins_h];
							this.head[this.ins_h] = (short)this.strstart;
						}
						while (--this.match_length != 0);
						this.strstart++;
					}
					else
					{
						this.strstart += this.match_length;
						this.match_length = 0;
						this.ins_h = (int)(this.window[this.strstart] & 255);
						this.ins_h = ((this.ins_h << this.hash_shift ^ (int)(this.window[this.strstart + 1] & 255)) & this.hash_mask);
					}
				}
				else
				{
					flag = this._tr_tally(0, (int)(this.window[this.strstart] & 255));
					this.lookahead--;
					this.strstart++;
				}
				if (flag)
				{
					this.flush_block_only(false);
					if (this.strm.avail_out == 0)
					{
						return 0;
					}
				}
			}
			return 0;
			IL_2C6:
			this.flush_block_only(flush == 4);
			if (this.strm.avail_out == 0)
			{
				if (flush == 4)
				{
					return 2;
				}
				return 0;
			}
			else
			{
				if (flush != 4)
				{
					return 1;
				}
				return 3;
			}
		}

		internal int deflate_slow(int flush)
		{
			int num = 0;
			while (true)
			{
				if (this.lookahead < 262)
				{
					this.fill_window();
					if (this.lookahead < 262 && flush == 0)
					{
						break;
					}
					if (this.lookahead == 0)
					{
						goto IL_325;
					}
				}
				if (this.lookahead >= 3)
				{
					this.ins_h = ((this.ins_h << this.hash_shift ^ (int)(this.window[this.strstart + 2] & 255)) & this.hash_mask);
					num = ((int)this.head[this.ins_h] & 65535);
					this.prev[this.strstart & this.w_mask] = this.head[this.ins_h];
					this.head[this.ins_h] = (short)this.strstart;
				}
				this.prev_length = this.match_length;
				this.prev_match = this.match_start;
				this.match_length = 2;
				if (num != 0 && this.prev_length < this.max_lazy_match && (this.strstart - num & 65535) <= this.w_size - 262)
				{
					if (this.strategy != 2)
					{
						this.match_length = this.longest_match(num);
					}
					if (this.match_length <= 5 && (this.strategy == 1 || (this.match_length == 3 && this.strstart - this.match_start > 4096)))
					{
						this.match_length = 2;
					}
				}
				if (this.prev_length >= 3 && this.match_length <= this.prev_length)
				{
					int num2 = this.strstart + this.lookahead - 3;
					bool flag = this._tr_tally(this.strstart - 1 - this.prev_match, this.prev_length - 3);
					this.lookahead -= this.prev_length - 1;
					this.prev_length -= 2;
					do
					{
						if (++this.strstart <= num2)
						{
							this.ins_h = ((this.ins_h << this.hash_shift ^ (int)(this.window[this.strstart + 2] & 255)) & this.hash_mask);
							num = ((int)this.head[this.ins_h] & 65535);
							this.prev[this.strstart & this.w_mask] = this.head[this.ins_h];
							this.head[this.ins_h] = (short)this.strstart;
						}
					}
					while (--this.prev_length != 0);
					this.match_available = 0;
					this.match_length = 2;
					this.strstart++;
					if (flag)
					{
						this.flush_block_only(false);
						if (this.strm.avail_out == 0)
						{
							return 0;
						}
					}
				}
				else if (this.match_available != 0)
				{
					bool flag = this._tr_tally(0, (int)(this.window[this.strstart - 1] & 255));
					if (flag)
					{
						this.flush_block_only(false);
					}
					this.strstart++;
					this.lookahead--;
					if (this.strm.avail_out == 0)
					{
						return 0;
					}
				}
				else
				{
					this.match_available = 1;
					this.strstart++;
					this.lookahead--;
				}
			}
			return 0;
			IL_325:
			if (this.match_available != 0)
			{
				bool flag = this._tr_tally(0, (int)(this.window[this.strstart - 1] & 255));
				this.match_available = 0;
			}
			this.flush_block_only(flush == 4);
			if (this.strm.avail_out == 0)
			{
				if (flush == 4)
				{
					return 2;
				}
				return 0;
			}
			else
			{
				if (flush != 4)
				{
					return 1;
				}
				return 3;
			}
		}

		internal int longest_match(int cur_match)
		{
			int num = this.max_chain_length;
			int num2 = this.strstart;
			int num3 = this.prev_length;
			int num4 = (this.strstart > this.w_size - 262) ? (this.strstart - (this.w_size - 262)) : 0;
			int num5 = this.nice_match;
			int num6 = this.w_mask;
			int num7 = this.strstart + 258;
			byte b = this.window[num2 + num3 - 1];
			byte b2 = this.window[num2 + num3];
			if (this.prev_length >= this.good_match)
			{
				num >>= 2;
			}
			if (num5 > this.lookahead)
			{
				num5 = this.lookahead;
			}
			do
			{
				int num8 = cur_match;
				if (this.window[num8 + num3] == b2 && this.window[num8 + num3 - 1] == b && this.window[num8] == this.window[num2] && this.window[++num8] == this.window[num2 + 1])
				{
					num2 += 2;
					num8++;
					while (this.window[++num2] == this.window[++num8] && this.window[++num2] == this.window[++num8] && this.window[++num2] == this.window[++num8] && this.window[++num2] == this.window[++num8] && this.window[++num2] == this.window[++num8] && this.window[++num2] == this.window[++num8] && this.window[++num2] == this.window[++num8] && this.window[++num2] == this.window[++num8] && num2 < num7)
					{
					}
					int num9 = 258 - (num7 - num2);
					num2 = num7 - 258;
					if (num9 > num3)
					{
						this.match_start = cur_match;
						num3 = num9;
						if (num9 >= num5)
						{
							break;
						}
						b = this.window[num2 + num3 - 1];
						b2 = this.window[num2 + num3];
					}
				}
			}
			while ((cur_match = ((int)this.prev[cur_match & num6] & 65535)) > num4 && --num != 0);
			if (num3 <= this.lookahead)
			{
				return num3;
			}
			return this.lookahead;
		}

		internal int deflateInit(ZStream strm, int level, int bits)
		{
			return this.deflateInit2(strm, level, 8, bits, 8, 0);
		}

		internal int deflateInit(ZStream strm, int level)
		{
			return this.deflateInit(strm, level, 15);
		}

		internal int deflateInit2(ZStream strm, int level, int method, int windowBits, int memLevel, int strategy)
		{
			int num = 0;
			strm.msg = null;
			if (level == -1)
			{
				level = 6;
			}
			if (windowBits < 0)
			{
				num = 1;
				windowBits = -windowBits;
			}
			if (memLevel < 1 || memLevel > 9 || method != 8 || windowBits < 9 || windowBits > 15 || level < 0 || level > 9 || strategy < 0 || strategy > 2)
			{
				return -2;
			}
			strm.dstate = this;
			this.noheader = num;
			this.w_bits = windowBits;
			this.w_size = 1 << this.w_bits;
			this.w_mask = this.w_size - 1;
			this.hash_bits = memLevel + 7;
			this.hash_size = 1 << this.hash_bits;
			this.hash_mask = this.hash_size - 1;
			this.hash_shift = (this.hash_bits + 3 - 1) / 3;
			this.window = new byte[this.w_size * 2];
			this.prev = new short[this.w_size];
			this.head = new short[this.hash_size];
			this.lit_bufsize = 1 << memLevel + 6;
			this.pending_buf = new byte[this.lit_bufsize * 4];
			this.pending_buf_size = this.lit_bufsize * 4;
			this.d_buf = this.lit_bufsize / 2;
			this.l_buf = 3 * this.lit_bufsize;
			this.level = level;
			this.strategy = strategy;
			this.method = (byte)method;
			return this.deflateReset(strm);
		}

		internal int deflateReset(ZStream strm)
		{
			strm.total_in = (strm.total_out = 0L);
			strm.msg = null;
			strm.data_type = 2;
			this.pending = 0;
			this.pending_out = 0;
			if (this.noheader < 0)
			{
				this.noheader = 0;
			}
			this.status = ((this.noheader != 0) ? 113 : 42);
			strm.adler = strm._adler.adler32(0L, null, 0, 0);
			this.last_flush = 0;
			this.tr_init();
			this.lm_init();
			return 0;
		}

		internal int deflateEnd()
		{
			if (this.status != 42 && this.status != 113 && this.status != 666)
			{
				return -2;
			}
			this.pending_buf = null;
			this.head = null;
			this.prev = null;
			this.window = null;
			if (this.status != 113)
			{
				return 0;
			}
			return -3;
		}

		internal int deflateParams(ZStream strm, int _level, int _strategy)
		{
			int result = 0;
			if (_level == -1)
			{
				_level = 6;
			}
			if (_level < 0 || _level > 9 || _strategy < 0 || _strategy > 2)
			{
				return -2;
			}
			if (Deflate.config_table[this.level].func != Deflate.config_table[_level].func && strm.total_in != 0L)
			{
				result = strm.deflate(1);
			}
			if (this.level != _level)
			{
				this.level = _level;
				this.max_lazy_match = Deflate.config_table[this.level].max_lazy;
				this.good_match = Deflate.config_table[this.level].good_length;
				this.nice_match = Deflate.config_table[this.level].nice_length;
				this.max_chain_length = Deflate.config_table[this.level].max_chain;
			}
			this.strategy = _strategy;
			return result;
		}

		internal int deflateSetDictionary(ZStream strm, byte[] dictionary, int dictLength)
		{
			int num = dictLength;
			int sourceIndex = 0;
			if (dictionary == null || this.status != 42)
			{
				return -2;
			}
			strm.adler = strm._adler.adler32(strm.adler, dictionary, 0, dictLength);
			if (num < 3)
			{
				return 0;
			}
			if (num > this.w_size - 262)
			{
				num = this.w_size - 262;
				sourceIndex = dictLength - num;
			}
			Array.Copy(dictionary, sourceIndex, this.window, 0, num);
			this.strstart = num;
			this.block_start = num;
			this.ins_h = (int)(this.window[0] & 255);
			this.ins_h = ((this.ins_h << this.hash_shift ^ (int)(this.window[1] & 255)) & this.hash_mask);
			for (int i = 0; i <= num - 3; i++)
			{
				this.ins_h = ((this.ins_h << this.hash_shift ^ (int)(this.window[i + 2] & 255)) & this.hash_mask);
				this.prev[i & this.w_mask] = this.head[this.ins_h];
				this.head[this.ins_h] = (short)i;
			}
			return 0;
		}

		internal int deflate(ZStream strm, int flush)
		{
			if (flush > 4 || flush < 0)
			{
				return -2;
			}
			if (strm.next_out == null || (strm.next_in == null && strm.avail_in != 0) || (this.status == 666 && flush != 4))
			{
				strm.msg = Deflate.z_errmsg[4];
				return -2;
			}
			if (strm.avail_out == 0)
			{
				strm.msg = Deflate.z_errmsg[7];
				return -5;
			}
			this.strm = strm;
			int num = this.last_flush;
			this.last_flush = flush;
			if (this.status == 42)
			{
				int num2 = 8 + (this.w_bits - 8 << 4) << 8;
				int num3 = (this.level - 1 & 255) >> 1;
				if (num3 > 3)
				{
					num3 = 3;
				}
				num2 |= num3 << 6;
				if (this.strstart != 0)
				{
					num2 |= 32;
				}
				num2 += 31 - num2 % 31;
				this.status = 113;
				this.putShortMSB(num2);
				if (this.strstart != 0)
				{
					this.putShortMSB((int)(strm.adler >> 16));
					this.putShortMSB((int)(strm.adler & 65535L));
				}
				strm.adler = strm._adler.adler32(0L, null, 0, 0);
			}
			if (this.pending != 0)
			{
				strm.flush_pending();
				if (strm.avail_out == 0)
				{
					this.last_flush = -1;
					return 0;
				}
			}
			else if (strm.avail_in == 0 && flush <= num && flush != 4)
			{
				strm.msg = Deflate.z_errmsg[7];
				return -5;
			}
			if (this.status == 666 && strm.avail_in != 0)
			{
				strm.msg = Deflate.z_errmsg[7];
				return -5;
			}
			if (strm.avail_in != 0 || this.lookahead != 0 || (flush != 0 && this.status != 666))
			{
				int num4 = -1;
				switch (Deflate.config_table[this.level].func)
				{
				case 0:
					num4 = this.deflate_stored(flush);
					break;
				case 1:
					num4 = this.deflate_fast(flush);
					break;
				case 2:
					num4 = this.deflate_slow(flush);
					break;
				}
				if (num4 == 2 || num4 == 3)
				{
					this.status = 666;
				}
				if (num4 == 0 || num4 == 2)
				{
					if (strm.avail_out == 0)
					{
						this.last_flush = -1;
					}
					return 0;
				}
				if (num4 == 1)
				{
					if (flush == 1)
					{
						this._tr_align();
					}
					else
					{
						this._tr_stored_block(0, 0, false);
						if (flush == 3)
						{
							for (int i = 0; i < this.hash_size; i++)
							{
								this.head[i] = 0;
							}
						}
					}
					strm.flush_pending();
					if (strm.avail_out == 0)
					{
						this.last_flush = -1;
						return 0;
					}
				}
			}
			if (flush != 4)
			{
				return 0;
			}
			if (this.noheader != 0)
			{
				return 1;
			}
			this.putShortMSB((int)(strm.adler >> 16));
			this.putShortMSB((int)(strm.adler & 65535L));
			strm.flush_pending();
			this.noheader = -1;
			if (this.pending == 0)
			{
				return 1;
			}
			return 0;
		}
	}
}
