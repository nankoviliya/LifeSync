export interface DonutSegment {
  value: number;
  /** a token-backed color: a CSS var() expression or hex resolved from a token — NOT a raw literal chosen ad hoc */
  color: string;
}

export interface DonutProps {
  segments: DonutSegment[];
  size?: number;
  stroke?: number;
  /** track color behind the segments */
  trackColor?: string;
}

// TODO(afterwork #5.charts): replace hand-rolled SVG with a real chart
// (animation, tooltips, possibly a charting lib). Values are real today.
export const Donut = ({
  segments,
  size = 120,
  stroke = 16,
  trackColor = 'var(--border)',
}: DonutProps) => {
  const radius = (size - stroke) / 2;
  const circumference = 2 * Math.PI * radius;
  const total = segments.reduce((sum, s) => sum + s.value, 0);

  let offset = 0;
  const arcs = segments.map((seg, i) => {
    const fraction = total > 0 ? seg.value / total : 0;
    const dash = fraction * circumference;
    const arc = (
      <circle
        key={i}
        cx={size / 2}
        cy={size / 2}
        r={radius}
        fill="none"
        stroke={seg.color}
        strokeWidth={stroke}
        strokeDasharray={`${dash} ${circumference - dash}`}
        strokeDashoffset={-offset}
      />
    );
    offset += dash;
    return arc;
  });

  return (
    <svg
      width={size}
      height={size}
      viewBox={`0 0 ${size} ${size}`}
      style={{ transform: 'rotate(-90deg)' }}
      aria-hidden="true"
    >
      <circle
        cx={size / 2}
        cy={size / 2}
        r={radius}
        fill="none"
        stroke={trackColor}
        strokeWidth={stroke}
      />
      {arcs}
    </svg>
  );
};
